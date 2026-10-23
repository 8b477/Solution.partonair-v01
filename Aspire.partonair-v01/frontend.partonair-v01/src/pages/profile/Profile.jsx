import { useContext, useEffect, useState } from "react";
import { useNavigate } from "react-router-dom";
import AuthContext from "../../contexts/auth/Auth-context";
import ProfileAvatar from "../../components/ui/Profile-avatar";
import { RecruiterNeedsBadges, RecruiterNeedsFields } from "../../components/ui/Recruiter-needs";
import EvaluationsSection from "../../components/ui/Evaluations-section";
import { StarRating } from "../../components/ui/Star-rating";
import { toRecruiterNeedsForm, toRecruiterNeedsPayload } from "../../constants/recruiter";
import { profileApi } from "../../services/api";

const ROLE_LABELS = {
	Employee: "Chercheur d’emploi",
	Company: "Recruteur / Entreprise",
};

const inputClass =
	"block w-full rounded-xl border border-white/10 bg-[#080E1B]/70 px-4 py-2.5 text-sm text-heading placeholder:text-body/70 focus:border-electric-300/60 focus:outline-none focus:ring-2 focus:ring-electric-500/30";

const cardTitleClass = "mb-4 text-xs font-semibold uppercase tracking-[0.2em] text-electric-300";

const toForm = (profile) => ({
	name: profile.userName,
	profileDescription: profile.profileDescription,
	skills: profile.skills,
	urlCv: profile.urlCv ?? "",
	recruiterNeeds: toRecruiterNeedsForm(profile.recruiterNeeds),
});

const toPayload = (profile, changes = {}) => ({
	name: profile.userName,
	profileDescription: profile.profileDescription,
	skills: profile.skills,
	urlCv: profile.urlCv || null,
	isPublic: profile.isPublic,
	showSelfInDirectory: profile.showSelfInDirectory,
	recruiterNeeds: profile.recruiterNeeds,
	...changes,
});

const errorMessage = (err, fallback) => {
	if (err.status === 401 || err.status === 403) return "Ta session a expiré. Reconnecte-toi.";
	if (err.status === 429) return err.serverMessage || "Limite journalière atteinte. Réessaie demain.";
	if (err.status === 400) return err.serverMessage || "Données invalides, vérifie les champs.";
	return fallback;
};

const Toggle = ({ id, checked, disabled, onChange, label, description }) => (
	<label htmlFor={id} className="flex cursor-pointer items-start justify-between gap-4">
		<span>
			<span className="block text-sm font-medium text-heading">{label}</span>
			<span className="mt-1 block text-xs leading-relaxed text-body">{description}</span>
		</span>
		<span className="relative mt-0.5 inline-flex shrink-0">
			<input
				id={id}
				type="checkbox"
				role="switch"
				checked={checked}
				disabled={disabled}
				onChange={(event) => onChange(event.target.checked)}
				className="peer sr-only"
			/>
			<span className="h-6 w-11 rounded-full border border-white/10 bg-white/10 transition peer-checked:bg-electric-500 peer-focus-visible:ring-2 peer-focus-visible:ring-electric-300 peer-disabled:opacity-60" />
			<span className="pointer-events-none absolute left-0.5 top-0.5 h-5 w-5 rounded-full bg-white shadow transition peer-checked:translate-x-5" />
		</span>
	</label>
);

const Profile = () => {
	const { auth, updateAuth } = useContext(AuthContext);
	const navigate = useNavigate();

	const [profile, setProfile] = useState(null);
	const [ratingStats, setRatingStats] = useState(null);
	const [loading, setLoading] = useState(true);
	const [loadError, setLoadError] = useState(null);

	const [editing, setEditing] = useState(false);
	const [form, setForm] = useState(null);
	const [saving, setSaving] = useState(false);
	const [regenerating, setRegenerating] = useState(false);
	const [feedback, setFeedback] = useState(null);

	useEffect(() => {
		if (!auth) {
			navigate("/");
			return;
		}
		if (!auth.profileId) {
			navigate("/profile/setup");
			return;
		}

		let cancelled = false;
		profileApi
			.getMe()
			.then((data) => {
				if (cancelled) return;
				setProfile(data.profile);
				// Sessions ouvertes avant l'ajout des avatars : on resynchronise la nav.
				if (data.profile.avatarSeed !== auth.avatarSeed || data.profile.role !== auth.role) {
					updateAuth({ avatarSeed: data.profile.avatarSeed, role: data.profile.role });
				}
			})
			.catch((err) => {
				if (cancelled) return;
				if (err.status === 404) navigate("/profile/setup");
				else setLoadError("Impossible de charger ton profil.");
			})
			.finally(() => {
				if (!cancelled) setLoading(false);
			});

		return () => {
			cancelled = true;
		};
		// Chargement unique par session : updateAuth modifie auth mais ne doit pas relancer la requête.
		// eslint-disable-next-line react-hooks/exhaustive-deps
	}, [auth?.userId, auth?.profileId, navigate]);

	const saveProfile = async (payload, successMessage) => {
		setSaving(true);
		setFeedback(null);
		try {
			const data = await profileApi.updateMe(payload);
			setProfile(data.profile);
			if (data.profile.userName !== auth.userName) updateAuth({ userName: data.profile.userName });
			setFeedback({ type: "success", message: successMessage });
			return true;
		} catch (err) {
			setFeedback({ type: "error", message: errorMessage(err, "La mise à jour a échoué. Réessaie.") });
			return false;
		} finally {
			setSaving(false);
		}
	};

	const startEditing = () => {
		setForm(toForm(profile));
		setFeedback(null);
		setEditing(true);
	};

	const handleChange = (event) => {
		const { name, value } = event.target;
		setForm((prev) => ({ ...prev, [name]: value }));
	};

	const handleSubmit = async (event) => {
		event.preventDefault();
		const saved = await saveProfile(
			toPayload(profile, {
				name: form.name.trim(),
				profileDescription: form.profileDescription.trim(),
				skills: form.skills.trim(),
				urlCv: form.urlCv.trim() || null,
				recruiterNeeds: profile.role === "Company" ? toRecruiterNeedsPayload(form.recruiterNeeds) : null,
			}),
			"Profil mis à jour.",
		);
		if (saved) setEditing(false);
	};

	const handleVisibility = (field, value) =>
		saveProfile(toPayload(profile, { [field]: value }), "Préférences de visibilité enregistrées.");

	const handleRegenerateAvatar = async () => {
		setRegenerating(true);
		setFeedback(null);
		try {
			const result = await profileApi.regenerateAvatar();
			setProfile((prev) => ({ ...prev, avatarSeed: result.avatarSeed, avatarRegenRemaining: result.remaining }));
			updateAuth({ avatarSeed: result.avatarSeed });
		} catch (err) {
			if (err.status === 429) setProfile((prev) => ({ ...prev, avatarRegenRemaining: 0 }));
			setFeedback({ type: "error", message: errorMessage(err, "Impossible de générer un nouvel avatar.") });
		} finally {
			setRegenerating(false);
		}
	};

	if (!auth) return null;

	if (loading) {
		return (
			<main className="flex min-h-screen items-center justify-center pt-24">
				<p className="text-body" role="status">Chargement…</p>
			</main>
		);
	}

	if (loadError || !profile) {
		return (
			<main className="flex min-h-screen items-center justify-center pt-24">
				<p className="text-red-300" role="alert">{loadError ?? "Profil introuvable."}</p>
			</main>
		);
	}

	const isCompany = profile.role === "Company";
	const skills = profile.skills
		.split(/[,;\n]/)
		.map((skill) => skill.trim())
		.filter(Boolean);

	return (
		<main className="min-h-screen px-4 pb-16 pt-28 text-left">
			<div className="mx-auto w-full max-w-5xl space-y-6">
				{/* En-tête */}
				<section className="glass-panel flex flex-col gap-6 rounded-2xl p-6 sm:flex-row sm:items-center sm:p-8">
					<div className="flex flex-col items-center gap-3">
						<ProfileAvatar
							name={profile.userName}
							seed={profile.avatarSeed}
							className="h-28 w-28 border-2 border-electric-300/40 text-3xl shadow-glow"
						/>
						<button
							type="button"
							onClick={handleRegenerateAvatar}
							disabled={regenerating || profile.avatarRegenRemaining <= 0}
							className="rounded-lg border border-white/10 px-3 py-1.5 text-xs font-medium text-body transition hover:border-electric-300/40 hover:text-heading disabled:cursor-not-allowed disabled:opacity-60">
							{regenerating ? "Génération…" : "🎲 Nouvel avatar"}
							<span className="ml-1 text-body/70">({profile.avatarRegenRemaining}/10)</span>
						</button>
					</div>

					<div className="min-w-0 flex-1 text-center sm:text-left">
						<p className="mb-2 text-xs font-semibold uppercase tracking-[0.2em] text-electric-300">Partonair · mon profil</p>
						<h1 className="truncate text-3xl font-bold text-heading">{profile.userName}</h1>
						<div className="mt-3 flex flex-wrap justify-center gap-2 sm:justify-start">
							<span className="rounded-full bg-electric-500/20 px-3 py-1 text-xs font-medium text-electric-300">
								{ROLE_LABELS[profile.role] ?? profile.role}
							</span>
							<span
								className={`rounded-full px-3 py-1 text-xs font-medium ${
									profile.isPublic ? "bg-green-500/20 text-green-300" : "bg-white/10 text-body"
								}`}>
								{profile.isPublic ? "Profil public" : "Profil privé"}
							</span>
						</div>
						<p className="mt-3 text-xs text-body">
							Membre depuis le {new Date(profile.profileCreatedAt).toLocaleDateString("fr-FR")}
						</p>
					</div>

					{!editing && (
						<button type="button" onClick={startEditing} className="button-secondary self-center sm:self-start">
							Modifier mon profil
						</button>
					)}
				</section>

				{feedback && (
					<p
						role={feedback.type === "error" ? "alert" : "status"}
						className={`rounded-xl border px-4 py-3 text-sm ${
							feedback.type === "error"
								? "border-red-400/30 bg-red-500/10 text-red-300"
								: "border-green-400/30 bg-green-500/10 text-green-300"
						}`}>
						{feedback.message}
					</p>
				)}

				<div className="grid grid-cols-1 gap-6 md:grid-cols-3">
					<div className="space-y-6 md:col-span-2">
						{editing ? (
							<form onSubmit={handleSubmit} className="glass-panel space-y-5 rounded-2xl p-6">
								<h2 className={cardTitleClass}>Modifier mon profil</h2>

								<div>
									<label htmlFor="name" className="mb-2 block text-sm font-medium text-heading">Nom affiché</label>
									<input id="name" name="name" value={form.name} onChange={handleChange} required minLength={2} maxLength={200} className={inputClass} />
								</div>

								<div>
									<label htmlFor="profileDescription" className="mb-2 block text-sm font-medium text-heading">
										Présentation <span className="text-xs text-body">(min. 20 caractères)</span>
									</label>
									<textarea
										id="profileDescription"
										name="profileDescription"
										value={form.profileDescription}
										onChange={handleChange}
										rows={5}
										required
										minLength={20}
										maxLength={1000}
										className={`${inputClass} resize-none`}
									/>
								</div>

								{isCompany && (
									<div className="space-y-3 border-t border-white/10 pt-5">
										<p className="text-sm font-semibold text-heading">Ce que je recherche</p>
										<RecruiterNeedsFields
											value={form.recruiterNeeds}
											onChange={(recruiterNeeds) => setForm((prev) => ({ ...prev, recruiterNeeds }))}
										/>
									</div>
								)}

								<div>
									<label htmlFor="skills" className="mb-2 block text-sm font-medium text-heading">
										{isCompany ? "Compétences recherchées" : "Compétences"}{" "}
										<span className="text-xs text-body">{isCompany ? "(optionnel, séparées par des virgules)" : "(séparées par des virgules)"}</span>
									</label>
									<input id="skills" name="skills" value={form.skills} onChange={handleChange} required={!isCompany} maxLength={500} className={inputClass} />
								</div>

								<div>
									<label htmlFor="urlCv" className="mb-2 block text-sm font-medium text-heading">
										Lien vers ton CV <span className="text-xs text-body">(optionnel)</span>
									</label>
									<input id="urlCv" name="urlCv" type="url" value={form.urlCv} onChange={handleChange} maxLength={500} placeholder="https://..." className={inputClass} />
								</div>

								<div className="flex flex-wrap justify-end gap-3 pt-2">
									<button
										type="button"
										onClick={() => setEditing(false)}
										disabled={saving}
										className="rounded-lg border border-white/10 px-4 py-2 text-sm font-medium text-body transition hover:border-white/20 hover:text-heading">
										Annuler
									</button>
									<button type="submit" disabled={saving} className="button-secondary disabled:opacity-60">
										{saving ? "Enregistrement…" : "Enregistrer"}
									</button>
								</div>
							</form>
						) : (
							<>
								<section className="glass-panel rounded-2xl p-6">
									<h2 className={cardTitleClass}>À propos</h2>
									<p className="whitespace-pre-line leading-relaxed text-heading">{profile.profileDescription}</p>
								</section>

								{isCompany && (
									<section className="glass-panel space-y-4 rounded-2xl p-6">
										<h2 className={cardTitleClass}>Ce que je recherche</h2>
										<p className="text-lg font-semibold text-heading">
											{profile.recruiterNeeds?.soughtPosition ?? "Poste non renseigné"}
										</p>
										<RecruiterNeedsBadges needs={profile.recruiterNeeds} />
									</section>
								)}

								{skills.length > 0 && (
									<section className="glass-panel rounded-2xl p-6">
										<h2 className={cardTitleClass}>{isCompany ? "Compétences recherchées" : "Compétences"}</h2>
										<ul className="flex flex-wrap gap-2">
											{skills.map((skill) => (
												<li key={skill} className="rounded-full border border-electric-300/30 bg-electric-500/10 px-3 py-1 text-sm text-heading">
													{skill}
												</li>
											))}
										</ul>
									</section>
								)}

								<div className="glass-panel rounded-2xl p-6">
									<EvaluationsSection userId={profile.userId} userName={profile.userName} readOnly onStatsChange={setRatingStats} />
								</div>
							</>
						)}
					</div>

					<aside className="space-y-6">
						<section className="glass-panel space-y-5 rounded-2xl p-6">
							<h2 className={cardTitleClass}>Visibilité</h2>
							<Toggle
								id="isPublic"
								checked={profile.isPublic}
								disabled={saving}
								onChange={(value) => handleVisibility("isPublic", value)}
								label="Profil public"
								description="Les autres membres te voient dans l’annuaire des profils."
							/>
							<Toggle
								id="showSelfInDirectory"
								checked={profile.showSelfInDirectory}
								disabled={saving}
								onChange={(value) => handleVisibility("showSelfInDirectory", value)}
								label="M’afficher dans mes recherches"
								description="Tu vois ta propre carte dans la liste des profils (si ton profil est public)."
							/>
						</section>

						<section className="glass-panel rounded-2xl p-6">
							<h2 className={cardTitleClass}>CV</h2>
							{profile.urlCv ? (
								<a
									href={profile.urlCv}
									target="_blank"
									rel="noopener noreferrer"
									className="text-sm text-electric-300 hover:text-electric-400 hover:underline">
									Voir mon CV ↗
								</a>
							) : (
								<p className="text-sm text-body">Aucun CV renseigné.</p>
							)}
						</section>

						<section className="glass-panel rounded-2xl p-6">
							<h2 className={cardTitleClass}>Évaluation</h2>
							<StarRating
								value={ratingStats?.average ?? profile.ratingAverage}
								count={ratingStats?.count ?? profile.ratingCount}
								className="text-xl"
							/>
						</section>
					</aside>
				</div>
			</div>
		</main>
	);
};

export default Profile;
