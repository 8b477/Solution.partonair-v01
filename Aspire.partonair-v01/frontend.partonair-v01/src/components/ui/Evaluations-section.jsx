import { useCallback, useContext, useEffect, useState } from "react";
import AuthContext from "../../contexts/auth/Auth-context";
import RealtimeContext from "../../contexts/realtime/Realtime-context";
import ProfileAvatar from "./Profile-avatar";
import { StarRating, StarRatingInput } from "./Star-rating";
import { evaluationApi } from "../../services/api";

const MIN_COMMENT = 10;
const MAX_COMMENT = 500;

const ROLE_LABELS = { Employee: "Chercheur d’emploi", Company: "Recruteur / Entreprise" };

const formatDate = (value) => new Date(value).toLocaleDateString("fr-FR", { day: "numeric", month: "long", year: "numeric" });

const errorMessage = (err) => {
	if (err.status === 409) return err.serverMessage || "Tu dois être ami avec ce membre pour l’évaluer.";
	if (err.status === 400) return "Vérifie la note et le commentaire (10 à 500 caractères).";
	return "Impossible d’enregistrer ton avis. Réessaie.";
};

const EvaluationCard = ({ evaluation, highlight }) => (
	<li className={`rounded-2xl border p-4 ${highlight ? "border-electric-300/40 bg-electric-500/10" : "border-white/10 bg-white/[0.03]"}`}>
		<div className="flex items-start gap-3">
			<ProfileAvatar name={evaluation.senderName} seed={evaluation.senderAvatarSeed} className="h-10 w-10 text-sm" />
			<div className="min-w-0 flex-1">
				<div className="flex flex-wrap items-center justify-between gap-x-3 gap-y-1">
					<p className="truncate text-sm font-semibold text-heading">
						{evaluation.senderName}
						{highlight && <span className="ml-2 text-xs font-medium text-electric-300">(ton avis)</span>}
					</p>
					<StarRating value={evaluation.value} className="text-sm" />
				</div>
				<p className="text-xs text-body">
					{ROLE_LABELS[evaluation.senderRole] ?? evaluation.senderRole} · {formatDate(evaluation.updatedAt ?? evaluation.createdAt)}
					{evaluation.updatedAt && " (modifié)"}
				</p>
				<p className="mt-2 whitespace-pre-line break-words text-sm leading-relaxed text-heading">{evaluation.commentary}</p>
			</div>
		</div>
	</li>
);

/**
 * Avis reçus par un membre. Sur la fiche d'un ami, permet aussi de laisser / modifier / supprimer son avis.
 * readOnly : page « Mon profil » (on ne s'évalue pas soi-même).
 */
const EvaluationsSection = ({ userId, userName, readOnly = false, onStatsChange }) => {
	const { auth } = useContext(AuthContext);
	const { subscribe } = useContext(RealtimeContext);

	const [data, setData] = useState(null);
	const [loading, setLoading] = useState(true);
	const [loadError, setLoadError] = useState(null);

	const [editing, setEditing] = useState(false);
	const [value, setValue] = useState(null);
	const [commentary, setCommentary] = useState("");
	const [saving, setSaving] = useState(false);
	const [formError, setFormError] = useState(null);

	const load = useCallback(() => {
		evaluationApi
			.getForUser(userId)
			.then((result) => {
				setData(result);
				setLoadError(null);
				onStatsChange?.({ average: result.average, count: result.count });
			})
			.catch(() => setLoadError("Impossible de charger les avis."))
			.finally(() => setLoading(false));
	}, [userId, onStatsChange]);

	useEffect(() => {
		if (userId) load();
	}, [userId, load]);

	// Ma propre fiche : nouveaux avis / suppressions en direct.
	useEffect(() => {
		if (!userId || userId !== auth?.userId) return;
		const unsubscribeReceived = subscribe("EvaluationReceived", load);
		const unsubscribeRemoved = subscribe("EvaluationRemoved", load);
		return () => {
			unsubscribeReceived();
			unsubscribeRemoved();
		};
	}, [userId, auth?.userId, subscribe, load]);

	const startEditing = () => {
		setValue(data?.myEvaluation?.value ?? null);
		setCommentary(data?.myEvaluation?.commentary ?? "");
		setFormError(null);
		setEditing(true);
	};

	const handleSubmit = async (event) => {
		event.preventDefault();
		if (value === null) {
			setFormError("Choisis une note entre 0 et 5.");
			return;
		}
		if (commentary.trim().length < MIN_COMMENT) {
			setFormError(`Explique ta note en au moins ${MIN_COMMENT} caractères.`);
			return;
		}

		setSaving(true);
		setFormError(null);
		try {
			await evaluationApi.upsert(userId, { value, commentary: commentary.trim() });
			setEditing(false);
			load();
		} catch (err) {
			setFormError(errorMessage(err));
		} finally {
			setSaving(false);
		}
	};

	const handleDelete = async () => {
		setSaving(true);
		setFormError(null);
		try {
			await evaluationApi.remove(userId);
			setEditing(false);
			load();
		} catch {
			setFormError("Impossible de supprimer ton avis. Réessaie.");
		} finally {
			setSaving(false);
		}
	};

	const myEvaluation = data?.myEvaluation;
	const others = (data?.evaluations ?? []).filter((evaluation) => evaluation.id !== myEvaluation?.id);
	const canEvaluate = !readOnly && data?.canEvaluate;

	return (
		<section className="space-y-5" aria-labelledby="evaluations-title">
			<div className="flex flex-wrap items-end justify-between gap-3">
				<div>
					<h2 id="evaluations-title" className="text-sm font-semibold uppercase tracking-wider text-body">
						Avis {data ? `(${data.count})` : ""}
					</h2>
					{data && (
						<div className="mt-2 flex items-baseline gap-3">
							{data.count > 0 && (
								<span className="text-3xl font-bold text-heading">
									{data.average.toLocaleString("fr-FR", { maximumFractionDigits: 1 })}
									<span className="text-base font-medium text-body">/5</span>
								</span>
							)}
							<StarRating value={data.average} count={data.count} className="text-xl" />
						</div>
					)}
				</div>

				{canEvaluate && !editing && (
					<button
						type="button"
						onClick={startEditing}
						className="rounded-lg bg-electric-500 px-4 py-2 text-sm font-semibold text-white transition hover:bg-electric-400">
						{myEvaluation ? "Modifier mon avis" : `Évaluer ${userName ?? "ce membre"}`}
					</button>
				)}
			</div>

			{!readOnly && data && !data.canEvaluate && !myEvaluation && auth?.userId !== userId && (
				<p className="text-xs text-body">Seuls les amis de ce membre peuvent laisser un avis.</p>
			)}

			{editing && (
				<form onSubmit={handleSubmit} className="space-y-4 rounded-2xl border border-electric-300/30 bg-electric-500/[0.07] p-5">
					<div>
						<p className="mb-2 text-sm font-medium text-heading">Ta note</p>
						<StarRatingInput value={value} onChange={setValue} disabled={saving} />
					</div>
					<div>
						<label htmlFor="evaluation-commentary" className="mb-2 block text-sm font-medium text-heading">
							Ton commentaire <span className="text-xs text-body">(justifie ta note, {MIN_COMMENT} à {MAX_COMMENT} caractères)</span>
						</label>
						<textarea
							id="evaluation-commentary"
							value={commentary}
							onChange={(event) => setCommentary(event.target.value)}
							rows={4}
							minLength={MIN_COMMENT}
							maxLength={MAX_COMMENT}
							required
							disabled={saving}
							placeholder="Comment s’est passée votre collaboration ?"
							className="block w-full resize-none rounded-xl border border-white/10 bg-[#080E1B]/70 px-4 py-2.5 text-sm text-heading placeholder:text-body/70 focus:border-electric-300/60 focus:outline-none focus:ring-2 focus:ring-electric-500/30"
						/>
						<p className="mt-1 text-right text-xs text-body">{commentary.length}/{MAX_COMMENT}</p>
					</div>
					{formError && <p className="text-sm text-red-300" role="alert">{formError}</p>}
					<div className="flex flex-wrap justify-between gap-3">
						{myEvaluation ? (
							<button
								type="button"
								onClick={handleDelete}
								disabled={saving}
								className="rounded-lg border border-red-400/30 px-4 py-2 text-sm font-medium text-red-300 transition hover:bg-red-500/10 disabled:opacity-60">
								Supprimer mon avis
							</button>
						) : (
							<span />
						)}
						<div className="flex gap-3">
							<button
								type="button"
								onClick={() => setEditing(false)}
								disabled={saving}
								className="rounded-lg border border-white/10 px-4 py-2 text-sm font-medium text-body transition hover:border-white/20 hover:text-heading">
								Annuler
							</button>
							<button
								type="submit"
								disabled={saving}
								className="rounded-lg bg-electric-500 px-4 py-2 text-sm font-semibold text-white transition hover:bg-electric-400 disabled:opacity-60">
								{saving ? "Enregistrement…" : "Publier mon avis"}
							</button>
						</div>
					</div>
				</form>
			)}

			{loading && <p className="text-sm text-body" role="status">Chargement des avis…</p>}
			{!loading && loadError && <p className="text-sm text-red-300" role="alert">{loadError}</p>}
			{!loading && !loadError && data?.count === 0 && !editing && (
				<p className="rounded-2xl border border-white/10 bg-white/[0.03] px-4 py-6 text-center text-sm text-body">
					Aucun avis pour le moment.
				</p>
			)}

			{!loading && !loadError && (myEvaluation || others.length > 0) && (
				<ul className="space-y-3">
					{myEvaluation && !editing && <EvaluationCard evaluation={myEvaluation} highlight />}
					{others.map((evaluation) => (
						<EvaluationCard key={evaluation.id} evaluation={evaluation} />
					))}
				</ul>
			)}
		</section>
	);
};

export default EvaluationsSection;
