import { useContext, useState } from "react";
import { useNavigate } from "react-router-dom";
import AuthContext from "../../contexts/auth/Auth-context";
import { profileApi, userApi } from "../../services/api";

const ROLES = [
	{ value: "Employee", label: "Chercheur d'emploi", description: "Tu recherches un poste ou une mission." },
	{ value: "Company", label: "Recruteur / Entreprise", description: "Tu recrutes des profils ou proposes des missions." },
];

const ProfileSetup = () => {
	const { auth, updateProfileId } = useContext(AuthContext);
	const navigate = useNavigate();

	const [form, setForm] = useState({
		role: "",
		profileDescription: "",
		skills: "",
		urlCv: "",
		isPublic: false,
	});
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
			};

			const result = await profileApi.create(auth.userId, profileData);
			const createdProfileId = result.profile.id;

			await userApi.changeRole(auth.userId, form.role);

			updateProfileId(createdProfileId);
			navigate("/profile");
		} catch (err) {
			if (err.status === 409) {
				setError("Tu as déjà un profil. Redirige toi vers ton profil.");
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
					<h1 className="text-3xl font-bold text-heading mb-2">Bienvenue, {auth.userName} !</h1>
					<p className="text-body">Dis-nous en plus sur toi pour compléter ton profil.</p>
				</div>

				<form onSubmit={handleSubmit} className="glass-panel p-8 rounded-xl space-y-6">
					{/* Choix du rôle */}
					<div>
						<p className="text-sm font-medium text-heading mb-3">Tu es...</p>
						<div className="grid grid-cols-1 sm:grid-cols-2 gap-4">
							{ROLES.map((r) => (
								<label
									key={r.value}
									className={`flex flex-col gap-1 p-4 rounded-lg border cursor-pointer transition-colors ${
										form.role === r.value
											? "border-brand bg-brand/10 text-heading"
											: "border-default-medium hover:border-brand/50 text-body"
									}`}>
									<input
										type="radio"
										name="role"
										value={r.value}
										checked={form.role === r.value}
										onChange={handleChange}
										className="sr-only"
									/>
									<span className="font-semibold text-heading">{r.label}</span>
									<span className="text-xs">{r.description}</span>
								</label>
							))}
						</div>
					</div>

					{/* Description */}
					<div>
						<label htmlFor="profileDescription" className="block mb-2 text-sm font-medium text-heading">
							Présente-toi <span className="text-body text-xs">(min. 20 caractères)</span>
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

					{/* Compétences */}
					<div>
						<label htmlFor="skills" className="block mb-2 text-sm font-medium text-heading">
							Tes compétences
						</label>
						<input
							type="text"
							id="skills"
							name="skills"
							value={form.skills}
							onChange={handleChange}
							required
							placeholder="Ex : React, Node.js, Gestion de projet..."
							className="bg-transparent border border-default-medium text-heading text-sm rounded-base focus:ring-brand focus:border-brand block w-full px-3 py-2.5 shadow-xs placeholder:text-body"
						/>
					</div>

					{/* URL CV */}
					<div>
						<label htmlFor="urlCv" className="block mb-2 text-sm font-medium text-heading">
							Lien vers ton CV <span className="text-body text-xs">(optionnel)</span>
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
						<label htmlFor="isPublic" className="text-sm text-heading cursor-pointer">
							Rendre mon profil visible par tous
						</label>
					</div>

					{error && <p className="text-red-400 text-sm">{error}</p>}

					<button type="submit" disabled={loading} className="button-primary w-full">
						{loading ? "Création en cours..." : "Créer mon profil"}
					</button>
				</form>
			</div>
		</main>
	);
};

export default ProfileSetup;
