import { useContext, useEffect, useState } from "react";
import { useNavigate } from "react-router-dom";
import AuthContext from "../../contexts/auth/Auth-context";
import { profileApi } from "../../services/api";

const ROLE_LABELS = {
	Employee: "Chercheur d'emploi",
	Company: "Recruteur / Entreprise",
	Visitor: "Visiteur",
};

const Profile = () => {
	const { auth, logout } = useContext(AuthContext);
	const navigate = useNavigate();

	const [profile, setProfile] = useState(null);
	const [loading, setLoading] = useState(true);
	const [error, setError] = useState(null);

	useEffect(() => {
		if (!auth) {
			navigate("/");
			return;
		}
		if (!auth.profileId) {
			navigate("/profile/setup");
			return;
		}

		profileApi
			.getById(auth.profileId)
			.then((data) => setProfile(data.profile))
			.catch(() => setError("Impossible de charger le profil."))
			.finally(() => setLoading(false));
	}, [auth, navigate]);

	const handleLogout = () => {
		logout();
		navigate("/");
	};

	if (loading) {
		return (
			<main className="min-h-screen pt-24 flex items-center justify-center">
				<p className="text-body">Chargement...</p>
			</main>
		);
	}

	if (error) {
		return (
			<main className="min-h-screen pt-24 flex items-center justify-center">
				<p className="text-red-400">{error}</p>
			</main>
		);
	}

	return (
		<main className="min-h-screen pt-24 pb-12 px-4 flex items-start justify-center">
			<div className="w-full max-w-2xl space-y-6">
				{/* Header */}
				<div className="glass-panel p-8 rounded-xl flex flex-col sm:flex-row items-start sm:items-center gap-6">
					<div className="w-16 h-16 rounded-full bg-brand/20 flex items-center justify-center text-2xl font-bold text-heading flex-shrink-0">
						{auth.userName?.charAt(0).toUpperCase()}
					</div>
					<div className="flex-1 min-w-0">
						<h1 className="text-2xl font-bold text-heading truncate">{auth.userName}</h1>
						<span className="inline-block mt-1 px-3 py-0.5 rounded-full text-xs font-medium bg-brand/20 text-fg-brand">
							{ROLE_LABELS[auth.role] ?? auth.role}
						</span>
						{profile?.isPublic && (
							<span className="ml-2 inline-block px-3 py-0.5 rounded-full text-xs font-medium bg-green-500/20 text-green-400">
								Profil public
							</span>
						)}
					</div>
					<button onClick={handleLogout} className="button-secondary text-sm flex-shrink-0">
						Déconnexion
					</button>
				</div>

				{/* Description */}
				{profile?.profileDescription && (
					<div className="glass-panel p-6 rounded-xl">
						<h2 className="text-sm font-semibold text-body uppercase tracking-wider mb-3">À propos</h2>
						<p className="text-heading leading-relaxed">{profile.profileDescription}</p>
					</div>
				)}

				{/* Compétences */}
				{profile?.skills && (
					<div className="glass-panel p-6 rounded-xl">
						<h2 className="text-sm font-semibold text-body uppercase tracking-wider mb-3">Compétences</h2>
						<p className="text-heading">{profile.skills}</p>
					</div>
				)}

				{/* CV */}
				{profile?.urlCv && (
					<div className="glass-panel p-6 rounded-xl">
						<h2 className="text-sm font-semibold text-body uppercase tracking-wider mb-3">CV</h2>
						<a
							href={profile.urlCv}
							target="_blank"
							rel="noopener noreferrer"
							className="text-fg-brand hover:underline hover:text-electric-300 text-sm">
							Voir mon CV
						</a>
					</div>
				)}

				{/* Évaluation */}
				{profile?.stars !== undefined && profile.stars > 0 && (
					<div className="glass-panel p-6 rounded-xl">
						<h2 className="text-sm font-semibold text-body uppercase tracking-wider mb-3">Évaluation</h2>
						<p className="text-heading text-lg font-semibold">{"★".repeat(profile.stars)}{"☆".repeat(5 - profile.stars)}</p>
					</div>
				)}
			</div>
		</main>
	);
};

export default Profile;
