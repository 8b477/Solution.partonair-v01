const MAX_STARS = 5;

const formatAverage = (value) => value.toLocaleString("fr-FR", { minimumFractionDigits: 1, maximumFractionDigits: 1 });

/** Affichage d'une note (décimales gérées : 3,5 = 3 étoiles et demie). */
export const StarRating = ({ value = 0, count, className = "text-base" }) => {
	const percent = Math.max(0, Math.min(100, (value / MAX_STARS) * 100));
	const label =
		count === 0 ? "Aucun avis" : `${formatAverage(value)} sur 5${count !== undefined ? `, ${count} avis` : ""}`;

	return (
		<span className={`inline-flex items-center gap-2 ${className}`} aria-label={label} title={label}>
			<span className="relative inline-block leading-none tracking-wider" aria-hidden="true">
				<span className="text-white/15">{"★".repeat(MAX_STARS)}</span>
				<span className="absolute inset-0 overflow-hidden whitespace-nowrap text-amber-300" style={{ width: `${percent}%` }}>
					{"★".repeat(MAX_STARS)}
				</span>
			</span>
			{count !== undefined && (
				<span className="text-xs text-body" aria-hidden="true">
					{count === 0 ? "Aucun avis" : `${formatAverage(value)} · ${count} avis`}
				</span>
			)}
		</span>
	);
};

/** Sélection d'une note entière de 0 à 5. */
export const StarRatingInput = ({ value, onChange, disabled }) => (
	<div role="radiogroup" aria-label="Note de 0 à 5" className="flex flex-wrap items-center gap-1">
		<button
			type="button"
			role="radio"
			aria-checked={value === 0}
			disabled={disabled}
			onClick={() => onChange(0)}
			className={`mr-1 rounded-full border px-2.5 py-1 text-xs font-semibold transition ${
				value === 0 ? "border-red-400/50 bg-red-500/15 text-red-200" : "border-white/10 text-body hover:text-heading"
			}`}>
			0
		</button>
		{Array.from({ length: MAX_STARS }, (_, index) => {
			const star = index + 1;
			return (
				<button
					key={star}
					type="button"
					role="radio"
					aria-checked={value === star}
					aria-label={`${star} étoile${star > 1 ? "s" : ""}`}
					disabled={disabled}
					onClick={() => onChange(star)}
					className={`text-2xl leading-none transition hover:scale-110 ${value !== null && star <= value ? "text-amber-300" : "text-white/20 hover:text-amber-200/60"}`}>
					★
				</button>
			);
		})}
		<span className="ml-2 text-sm text-body">{value === null ? "Choisis une note" : `${value}/5`}</span>
	</div>
);
