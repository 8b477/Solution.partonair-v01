import { useContext, useEffect, useState } from "react";
import { Link, useNavigate } from "react-router-dom";
import AuthContext from "../../contexts/auth/Auth-context";
import ProfileAvatar from "../../components/ui/Profile-avatar";
import { RecruiterNeedsBadges } from "../../components/ui/Recruiter-needs";
import { StarRating } from "../../components/ui/Star-rating";
import { profileApi } from "../../services/api";

const TABS = [
    { value: "Employee", label: "Chercheurs d’emploi" },
    { value: "Company", label: "Recruteurs / Entreprises" },
];

const Profiles = () => {
    const { auth } = useContext(AuthContext);
    const navigate = useNavigate();

    const [activeRole, setActiveRole] = useState("Employee");
    const [search, setSearch] = useState("");
    const [profiles, setProfiles] = useState([]);
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState(null);

    useEffect(() => {
        if (!auth) {
            navigate("/");
            return;
        }
    }, [auth, navigate]);

    useEffect(() => {
        if (!auth) return;

        let cancelled = false;
        Promise.resolve().then(() => {
            if (!cancelled) {
                setLoading(true);
                setError(null);
            }
        });

        profileApi
            .getByRole(activeRole)
            .then((data) => {
                if (!cancelled) setProfiles(data?.profiles ?? []);
            })
            .catch((err) => {
                if (cancelled) return;
                setError(err.status === 401 || err.status === 403
                    ? "Connecte-toi pour consulter les profils."
                    : "Impossible de charger les profils. Réessaie.");
                setProfiles([]);
            })
            .finally(() => {
                if (!cancelled) setLoading(false);
            });

        return () => {
            cancelled = true;
        };
    }, [auth, activeRole]);

    const normalizedSearch = search.trim().toLocaleLowerCase("fr");
    const filteredProfiles = profiles.filter((profile) =>
        [profile.userName, profile.profileDescription, profile.skills, profile.recruiterNeeds?.soughtPosition, profile.recruiterNeeds?.location]
            .some((value) => value?.toLocaleLowerCase("fr").includes(normalizedSearch)));

    if (!auth) return null;

    return (
        <main className="min-h-screen px-4 pb-16 pt-28 text-left">
            <div className="mx-auto w-full max-w-6xl">
                <header className="mx-auto mb-8 max-w-2xl text-center">
                    <p className="mb-3 text-xs font-semibold uppercase tracking-[0.2em] text-electric-300">Partonair · communauté</p>
                    <h1 className="text-balance text-3xl font-bold leading-tight text-heading sm:text-4xl">Trouve les bons profils</h1>
                    <p className="mt-3 text-base leading-relaxed text-body">Découvre les personnes et recruteurs qui font avancer tes projets.</p>
                </header>

                <section className="glass-panel mb-8 rounded-2xl p-4 sm:p-5" aria-label="Filtres de profils">
                    <div className="grid grid-cols-1 gap-2 sm:grid-cols-2" role="tablist" aria-label="Type de profil">
                        {TABS.map((tab) => {
                            const selected = activeRole === tab.value;
                            return (
                                <button
                                    key={tab.value}
                                    type="button"
                                    role="tab"
                                    aria-selected={selected}
                                    onClick={() => setActiveRole(tab.value)}
                                    className={`rounded-xl px-4 py-3 text-sm font-semibold transition focus-visible:outline-none focus-visible:ring-2 focus-visible:ring-electric-300 ${selected
                                        ? "border border-electric-300/40 bg-electric-500/20 text-white shadow-glow"
                                        : "border border-white/10 bg-white/[0.03] text-body hover:border-white/20 hover:bg-white/[0.07] hover:text-heading"
                                    }`}>
                                    {tab.label}
                                </button>
                            );
                        })}
                    </div>

                    <label htmlFor="profile-search" className="sr-only">Rechercher dans la liste affichée</label>
                    <div className="relative mt-4">
                        <svg className="pointer-events-none absolute left-4 top-1/2 h-5 w-5 -translate-y-1/2 text-body" viewBox="0 0 24 24" fill="none" aria-hidden="true">
                            <circle cx="11" cy="11" r="7" stroke="currentColor" strokeWidth="1.8" />
                            <path d="m16 16 4 4" stroke="currentColor" strokeWidth="1.8" strokeLinecap="round" />
                        </svg>
                        <input
                            id="profile-search"
                            type="search"
                            value={search}
                            onChange={(event) => setSearch(event.target.value)}
                            placeholder={activeRole === "Company" ? "Rechercher une entreprise…" : "Rechercher un chercheur d’emploi…"}
                            className="block w-full rounded-xl border border-white/10 bg-[#080E1B]/70 py-3 pl-12 pr-4 text-sm text-heading placeholder:text-body/70 focus:border-electric-300/60 focus:outline-none focus:ring-2 focus:ring-electric-500/30"
                        />
                    </div>
                    <p className="mt-3 text-right text-xs text-body" aria-live="polite">
                        {loading ? "Chargement…" : `${filteredProfiles.length} profil${filteredProfiles.length > 1 ? "s" : ""}`}
                    </p>
                </section>

                {loading && <p className="py-16 text-center text-body" role="status">Chargement des profils…</p>}
                {!loading && error && <p className="glass-panel rounded-2xl p-8 text-center text-red-300" role="alert">{error}</p>}
                {!loading && !error && profiles.length > 0 && filteredProfiles.length === 0 && (
                    <div className="glass-panel rounded-2xl px-6 py-12 text-center">
                        <h2 className="text-lg font-semibold text-heading">Aucun résultat pour cette recherche</h2>
                        <p className="mt-2 text-sm text-body">Essaie un autre nom, une compétence ou un mot-clé.</p>
                    </div>
                )}
                {!loading && !error && profiles.length === 0 && (
                    <div className="glass-panel rounded-2xl px-6 py-12 text-center">
                        <h2 className="text-lg font-semibold text-heading">La communauté s’agrandit</h2>
                        <p className="mt-2 text-sm text-body">Aucun profil public dans cette catégorie pour le moment.</p>
                    </div>
                )}

                {!loading && !error && filteredProfiles.length > 0 && (
                    <ul className="grid grid-cols-1 gap-5 md:grid-cols-2 xl:grid-cols-3" aria-label="Profils publics">
                        {filteredProfiles.map((profile) => (
                            <li key={profile.profileId}>
                                <article className="glass-panel group h-full overflow-hidden rounded-2xl transition duration-200 hover:-translate-y-1 hover:border-electric-300/30 hover:bg-white/[0.08] hover:shadow-glow focus-within:ring-2 focus-within:ring-electric-300">
                                    <Link to={`/profiles/${profile.profileId}`} className="flex h-full flex-col p-5 text-left focus-visible:outline-none sm:p-6">
                                        <div className="flex items-center gap-4">
                                            <ProfileAvatar name={profile.userName} seed={profile.avatarSeed} className="h-14 w-14 text-lg" />
                                            <div className="min-w-0 flex-1">
                                                <h2 className="truncate text-lg font-semibold text-heading">{profile.userName}</h2>
                                                <span className="mt-1 inline-flex rounded-full border border-electric-300/20 bg-electric-500/10 px-2.5 py-1 text-xs font-medium text-electric-300">
                                                    {profile.role === "Company" ? "Recruteur / Entreprise" : "Chercheur d’emploi"}
                                                </span>
                                            </div>
                                        </div>
                                        <p className="mt-5 line-clamp-3 text-sm leading-relaxed text-body">{profile.profileDescription}</p>
                                        {profile.role === "Company" ? (
                                            <div className="mt-5 space-y-3">
                                                <div>
                                                    <h3 className="text-xs font-semibold uppercase tracking-wider text-body">Recherche</h3>
                                                    <p className="mt-1 line-clamp-2 text-sm font-medium text-heading">
                                                        {profile.recruiterNeeds?.soughtPosition ?? "Poste non renseigné"}
                                                    </p>
                                                </div>
                                                <RecruiterNeedsBadges needs={profile.recruiterNeeds} />
                                            </div>
                                        ) : (
                                            <div className="mt-5">
                                                <h3 className="text-xs font-semibold uppercase tracking-wider text-body">Compétences</h3>
                                                <p className="mt-1 line-clamp-2 text-sm text-heading">{profile.skills}</p>
                                            </div>
                                        )}
                                        <div className="mt-auto flex items-center justify-between border-t border-white/10 pt-5">
                                            <StarRating value={profile.ratingAverage} count={profile.ratingCount} className="text-sm" />
                                            <span className="text-sm font-semibold text-electric-300 transition group-hover:text-white">Voir le profil <span aria-hidden="true">→</span></span>
                                        </div>
                                    </Link>
                                </article>
                            </li>
                        ))}
                    </ul>
                )}
            </div>
        </main>
    );
};

export default Profiles;