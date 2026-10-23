import { useContext, useState } from "react";
import { useNavigate } from "react-router-dom";
import AuthContext from "../../contexts/auth/Auth-context";
import { profileApi } from "../../services/api";
import { RecruiterNeedsFields } from "../../components/ui/Recruiter-needs";
import { EMPTY_RECRUITER_NEEDS, toRecruiterNeedsPayload } from "../../constants/recruiter";

    const ROLES = [
	    { value: "Employee", label: "Chercheur d'emploi", description: "Tu recherches un poste ou une mission." },
	    { value: "Company", label: "Recruteur / Entreprise", description: "Tu recrutes des profils ou proposes des missions." },
    ];

    const ProfileSetup = () => {
        const { auth, updateAuth } = useContext(AuthContext);
        const navigate = useNavigate();

        const [form, setForm] = useState({
            role: "",
            profileDescription: "",
            skills: "",
            urlCv: "",
            isPublic: false,
        });
        const [recruiterNeeds, setRecruiterNeeds] = useState(EMPTY_RECRUITER_NEEDS);


    const [error, setError] = useState(null);
    const [loading, setLoading] = useState(false);

    if (!auth) {
        navigate("/");
        return null;
    }

    const handleChange = (e) => {
        const { name, value, type, checked } = e.target;
        setForm((prev) => ({ ...prev, [name]: type === "checkbox" ? checked : value }));
        setError(null);
    };

    const isCompany = form.role === "Company";

    const handleSubmit = async (e) => {
        e.preventDefault();
        setError(null);

        if (!form.role) {
            setError("Choisis ton profil (chercheur d'emploi ou recruteur).");
            return;
        }

        setLoading(true);
        try {
            const profileData = {
                profileDescription: form.profileDescription,
                skills: form.skills,
                urlCv: form.urlCv || null,
                isPublic: form.isPublic,
                role: form.role,
                recruiterNeeds: isCompany ? toRecruiterNeedsPayload(recruiterNeeds) : null,
            };

            const result = await profileApi.create(auth.userId, profileData);
            const createdProfile = result.profile;

            updateAuth({ profileId: createdProfile.id, role: form.role, avatarSeed: createdProfile.avatarSeed });
            navigate("/profile");
        } catch (err) {
            if (err.status === 409) {
                setError("Tu as déjà un profil. Redirige-toi vers tes profils.");
            } else if (err.status === 400) {
                setError(err.serverMessage || "Données invalides, vérifie les champs.");
            } else if (err.status === 401 || err.status === 403) {
                setError("Ta session a expiré. Reconnecte-toi.");
            } else {
                setError("Une erreur est survenue. Réessaie.");
            }
        } finally {
            setLoading(false);
        }
    };

    return (
        <main className="min-h-screen pt-24 pb-12 px-4 flex items-start justify-center">
            <div className="w-full max-w-2xl">
                <div className="mb-8 text-center">
                    <h1 className="text-3xl font-bold text-heading mb-2">
                        Bienvenue, {auth.userName} !
                    </h1>
                    <p className="text-body">
                        Dis-nous en plus sur toi pour compléter ton profil.
                    </p>
                </div>

                <form
                    onSubmit={handleSubmit}
                    className="glass-panel p-8 rounded-xl space-y-6"
                >
                    {/* Choix du rôle */}
                    <div>
                        <p className="text-sm font-medium text-heading mb-3">Tu es...</p>

                        <div className="grid grid-cols-1 sm:grid-cols-2 gap-4">
                            {ROLES.map((r) => {
                                const isSelected = form.role === r.value;

                                return (
                                    <label
                                        key={r.value}
                                        className={`relative flex flex-col gap-2 p-4 rounded-xl border-2 cursor-pointer transition-all duration-200 ${isSelected
                                            ? "border-blue-500 bg-blue-50 ring-2 ring-blue-200"
                                            : "border-gray-300 bg-white hover:border-blue-300 hover:bg-gray-50"
                                            }`}
                                    >
                                        <input
                                            type="radio"
                                            name="role"
                                            value={r.value}
                                            checked={isSelected}
                                            onChange={handleChange}
                                            className="sr-only"
                                        />

                                        {isSelected && (
                                            <span className="absolute top-3 right-3 flex h-5 w-5 items-center justify-center rounded-full bg-blue-500 text-white text-xs">
                                                ✓
                                            </span>
                                        )}

                                        <span
                                            className={`font-semibold ${isSelected ? "text-blue-700" : "text-gray-900"
                                                }`}
                                        >
                                            {r.label}
                                        </span>

                                        <span
                                            className={`text-xs ${isSelected ? "text-blue-600" : "text-gray-500"
                                                }`}
                                        >
                                            {r.description}
                                        </span>
                                    </label>
                                );
                            })}
                        </div>
                    </div>

                    {/* Description */}
                    <div>
                        <label
                            htmlFor="profileDescription"
                            className="block mb-2 text-sm font-medium text-heading"
                        >
                            Présente-toi{" "}
                            <span className="text-body text-xs">(min. 20 caractères)</span>
                        </label>

                        <textarea
                            id="profileDescription"
                            name="profileDescription"
                            value={form.profileDescription}
                            onChange={handleChange}
                            rows={4}
                            minLength={20}
                            required
                            placeholder="Parle de ton parcours, de tes objectifs..."
                            className="bg-transparent border border-default-medium text-heading text-sm rounded-base focus:ring-brand focus:border-brand block w-full px-3 py-2.5 shadow-xs placeholder:text-body resize-none"
                        />
                    </div>

                    {/* Besoins du recruteur */}
                    {isCompany && (
                        <div className="space-y-3 border-t border-white/10 pt-6">
                            <p className="text-sm font-semibold text-heading">Ce que tu recherches</p>
                            <RecruiterNeedsFields value={recruiterNeeds} onChange={setRecruiterNeeds} />
                        </div>
                    )}

                    {/* Compétences */}
                    <div>
                        <label
                            htmlFor="skills"
                            className="block mb-2 text-sm font-medium text-heading"
                        >
                            {isCompany ? "Compétences recherchées" : "Tes compétences"}{" "}
                            <span className="text-body text-xs">{isCompany ? "(optionnel, séparées par des virgules)" : "(séparées par des virgules)"}</span>
                        </label>

                        <input
                            type="text"
                            id="skills"
                            name="skills"
                            value={form.skills}
                            onChange={handleChange}
                            required={!isCompany}
                            maxLength={500}
                            placeholder="Ex : React, Node.js, Gestion de projet..."
                            className="bg-transparent border border-default-medium text-heading text-sm rounded-base focus:ring-brand focus:border-brand block w-full px-3 py-2.5 shadow-xs placeholder:text-body"
                        />
                    </div>

                    {/* URL CV */}
                    <div>
                        <label
                            htmlFor="urlCv"
                            className="block mb-2 text-sm font-medium text-heading"
                        >
                            Lien vers ton CV{" "}
                            <span className="text-body text-xs">(optionnel)</span>
                        </label>

                        <input
                            type="url"
                            id="urlCv"
                            name="urlCv"
                            value={form.urlCv}
                            onChange={handleChange}
                            placeholder="https://..."
                            className="bg-transparent border border-default-medium text-heading text-sm rounded-base focus:ring-brand focus:border-brand block w-full px-3 py-2.5 shadow-xs placeholder:text-body"
                        />
                    </div>

                    {/* Profil public */}
                    <div className="flex items-center gap-3">
                        <input
                            type="checkbox"
                            id="isPublic"
                            name="isPublic"
                            checked={form.isPublic}
                            onChange={handleChange}
                            className="w-4 h-4 accent-brand"
                        />

                        <label
                            htmlFor="isPublic"
                            className="text-sm text-heading cursor-pointer"
                        >
                            Rendre mon profil visible par tous
                        </label>
                    </div>

                    {error && <p className="text-red-400 text-sm">{error}</p>}

                    <button
                        type="submit"
                        disabled={loading}
                        className="button-primary w-full"
                    >
                        {loading ? "Création en cours..." : "Créer mon profil"}
                    </button>
                </form>
            </div>
        </main>
    );
}
export default ProfileSetup;
