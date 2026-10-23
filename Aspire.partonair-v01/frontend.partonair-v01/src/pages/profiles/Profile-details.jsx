import { useCallback, useContext, useEffect, useState } from "react";
import { Link, useNavigate, useParams } from "react-router-dom";
import AuthContext from "../../contexts/auth/Auth-context";
import NotificationsContext from "../../contexts/notifications/Notifications-context";
import RealtimeContext from "../../contexts/realtime/Realtime-context";
import ProfileAvatar from "../../components/ui/Profile-avatar";
import { RecruiterNeedsBadges } from "../../components/ui/Recruiter-needs";
import EvaluationsSection from "../../components/ui/Evaluations-section";
import { StarRating } from "../../components/ui/Star-rating";
import { contactApi, profileApi } from "../../services/api";

const primaryButton =
    "rounded-lg bg-electric-500 px-4 py-2 text-sm font-semibold text-white transition hover:bg-electric-400 disabled:cursor-not-allowed disabled:opacity-60";
const secondaryButton =
    "rounded-lg border border-white/10 px-4 py-2 text-sm font-medium text-body transition hover:border-white/20 hover:text-heading disabled:cursor-not-allowed disabled:opacity-60";

const ProfileDetails = () => {
    const { auth } = useContext(AuthContext);
    const { refreshIncoming } = useContext(NotificationsContext);
    const { subscribe } = useContext(RealtimeContext);
    const { profileId } = useParams();
    const navigate = useNavigate();
    const [profile, setProfile] = useState(null);
    // Moyenne à jour après publication d'un avis (sinon celle chargée avec le profil).
    const [ratingStats, setRatingStats] = useState(null);
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState(null);
    // Relation avec ce profil : { status, contactId } (voir RelationshipStatus côté API).
    const [relationship, setRelationship] = useState(null);
    const [actionState, setActionState] = useState("idle");

    useEffect(() => {
        if (!auth) {
            navigate("/");
            return;
        }

        let cancelled = false;
        Promise.resolve().then(() => {
            if (!cancelled) {
                setLoading(true);
                setError(null);
            }
        });

        profileApi
            .getPublicById(profileId)
            .then((data) => {
                if (!cancelled) setProfile(data?.profile ?? null);
            })
            .catch((err) => {
                if (cancelled) return;
                setProfile(null);
                setError(err.status === 404
                    ? "Ce profil n’existe pas ou n’est pas public."
                    : "Impossible de charger ce profil. Réessaie.");
            })
            .finally(() => {
                if (!cancelled) setLoading(false);
            });

        return () => {
            cancelled = true;
        };
    }, [auth, navigate, profileId]);

    const otherUserId = profile?.userId;
    const isOwnProfile = otherUserId === auth?.userId;

    const loadRelationship = useCallback(() => {
        if (!otherUserId || isOwnProfile) return;
        contactApi
            .getRelationship(otherUserId)
            .then(setRelationship)
            .catch(() => setRelationship(null));
    }, [otherUserId, isOwnProfile]);

    useEffect(() => {
        loadRelationship();
    }, [loadRelationship]);

    // Blocage / déblocage fait ailleurs (autre onglet ou l'autre utilisateur).
    useEffect(() => {
        if (!otherUserId) return;
        return subscribe("RelationshipChanged", ({ userId }) => {
            if (userId === otherUserId) loadRelationship();
        });
    }, [otherUserId, subscribe, loadRelationship]);

    const runAction = async (action) => {
        if (actionState === "pending") return;
        setActionState("pending");
        try {
            await action();
            setActionState("idle");
        } catch {
            setActionState("error");
        } finally {
            loadRelationship();
            refreshIncoming();
        }
    };

    const renderRelationshipActions = () => {
        if (!relationship) return null;
        const pending = actionState === "pending";

        switch (relationship.status) {
            case "None":
                return (
                    <button type="button" disabled={pending} onClick={() => runAction(() => contactApi.sendRequest(otherUserId))} className={primaryButton}>
                        {pending ? "Envoi…" : "Ajouter en ami"}
                    </button>
                );
            case "PendingOutgoing":
                return <button type="button" disabled className={secondaryButton}>Demande envoyée</button>;
            case "PendingIncoming":
                return (
                    <>
                        <button type="button" disabled={pending} onClick={() => runAction(() => contactApi.acceptRequest(relationship.contactId))} className={primaryButton}>
                            Accepter sa demande
                        </button>
                        <button type="button" disabled={pending} onClick={() => runAction(() => contactApi.refuseRequest(relationship.contactId))} className={secondaryButton}>
                            Refuser
                        </button>
                    </>
                );
            case "Friends":
                return (
                    <>
                        <span className="rounded-full border border-green-400/30 bg-green-500/10 px-3 py-1 text-xs font-medium text-green-300">✓ Vous êtes amis</span>
                        <button type="button" onClick={() => navigate(`/messages/${otherUserId}`)} className={primaryButton}>
                            Envoyer un message
                        </button>
                    </>
                );
            case "Refused":
                return <p className="text-sm text-body">Une demande a déjà été refusée entre vous.</p>;
            case "BlockedByMe":
                return (
                    <>
                        <span className="text-sm text-red-300">🔒 Tu as bloqué ce profil.</span>
                        <button type="button" disabled={pending} onClick={() => runAction(() => contactApi.unblock(otherUserId))} className={secondaryButton}>
                            {pending ? "Déblocage…" : "Débloquer"}
                        </button>
                    </>
                );
            case "BlockedByOther":
                // Seul celui qui a bloqué peut lever le blocage : aucune action possible ici.
                return <p className="text-sm text-red-300">🔒 Ce membre t’a bloqué : tu ne peux pas lui envoyer de demande.</p>;
            default:
                return null;
        }
    };

    if (!auth) return null;

    return (
        <main className="min-h-screen px-4 pb-16 pt-28 text-left">
            <div className="mx-auto w-full max-w-3xl">
                <Link to="/profiles" className="mb-6 inline-flex items-center gap-2 text-sm text-body transition hover:text-white focus-visible:outline-none focus-visible:ring-2 focus-visible:ring-electric-300">
                    <span aria-hidden="true">←</span>
                    Retour à l’annuaire
                </Link>

                {loading && <p className="py-16 text-center text-body" role="status">Chargement du profil…</p>}
                {!loading && error && <p className="glass-panel rounded-2xl p-8 text-center text-red-300" role="alert">{error}</p>}

                {!loading && profile && (
                    <article className="glass-panel overflow-hidden rounded-3xl">
                        <div className="bg-gradient-to-r from-electric-600/25 via-indigo-500/15 to-transparent p-6 sm:p-10">
                            <div className="flex flex-col items-start gap-5 sm:flex-row sm:items-center">
                                <ProfileAvatar name={profile.userName} seed={profile.avatarSeed} className="h-20 w-20 text-2xl" />
                                <div className="min-w-0">
                                    <h1 className="text-balance text-3xl font-bold leading-tight text-heading">{profile.userName}</h1>
                                    <p className="mt-2 inline-flex rounded-full border border-electric-300/20 bg-electric-500/15 px-3 py-1 text-xs font-medium tracking-wide text-electric-300">
                                        {profile.role === "Company" ? "Recruteur / Entreprise" : "Chercheur d’emploi"}
                                    </p>
                                    <div className="mt-3">
                                        <StarRating
                                            value={ratingStats?.average ?? profile.ratingAverage}
                                            count={ratingStats?.count ?? profile.ratingCount}
                                            className="text-lg"
                                        />
                                    </div>
                                </div>
                            </div>
                        </div>

                        <div className="space-y-8 p-6 sm:p-10">
                            {!isOwnProfile && (
                                <div>
                                    <div className="flex flex-wrap items-center gap-3">{renderRelationshipActions()}</div>
                                    {actionState === "error" && <p className="mt-2 text-sm text-red-300">Action impossible, réessaie.</p>}
                                </div>
                            )}
                            <section>
                                <h2 className="mb-3 text-sm font-semibold uppercase tracking-wider text-body">Présentation</h2>
                                <p className="max-w-prose whitespace-pre-line text-base leading-relaxed text-heading">{profile.profileDescription}</p>
                            </section>

                            {profile.role === "Company" && (
                                <section className="space-y-3">
                                    <h2 className="text-sm font-semibold uppercase tracking-wider text-body">Ce que je recherche</h2>
                                    <p className="text-lg font-semibold text-heading">
                                        {profile.recruiterNeeds?.soughtPosition ?? "Poste non renseigné"}
                                    </p>
                                    <RecruiterNeedsBadges needs={profile.recruiterNeeds} />
                                </section>
                            )}

                            {profile.skills && (
                                <section>
                                    <h2 className="mb-3 text-sm font-semibold uppercase tracking-wider text-body">
                                        {profile.role === "Company" ? "Compétences recherchées" : "Compétences"}
                                    </h2>
                                    <p className="whitespace-pre-line text-base leading-relaxed text-heading">{profile.skills}</p>
                                </section>
                            )}

                            {profile.urlCv && (
                                <a href={profile.urlCv} target="_blank" rel="noopener noreferrer" className="inline-flex rounded-lg border border-electric-300/30 px-4 py-2 text-sm font-medium text-electric-300 transition hover:bg-electric-500/15 focus-visible:outline-none focus-visible:ring-2 focus-visible:ring-electric-300">
                                    Consulter le CV
                                </a>
                            )}

                            <div className="border-t border-white/10 pt-8">
                                <EvaluationsSection
                                    userId={profile.userId}
                                    userName={profile.userName}
                                    readOnly={isOwnProfile}
                                    onStatsChange={setRatingStats}
                                />
                            </div>
                        </div>
                    </article>
                )}
            </div>
        </main>
    );
};

export default ProfileDetails;
