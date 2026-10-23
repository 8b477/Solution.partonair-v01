import { CONTRACT_TYPES, REMOTE_POLICIES, SECTORS, labelOf } from "../../constants/recruiter";

const inputClass =
	"block w-full rounded-xl border border-white/10 bg-[#080E1B]/70 px-4 py-2.5 text-sm text-heading placeholder:text-body/70 focus:border-electric-300/60 focus:outline-none focus:ring-2 focus:ring-electric-500/30";

const chipClass = (selected) =>
	`cursor-pointer rounded-full border px-3 py-1.5 text-sm transition ${
		selected
			? "border-electric-300/60 bg-electric-500/25 text-white"
			: "border-white/10 bg-white/[0.03] text-body hover:border-white/20 hover:text-heading"
	}`;

/** Champs « Ce que je recherche » d'un recruteur (formulaire contrôlé). */
export const RecruiterNeedsFields = ({ value, onChange }) => {
	const update = (field, fieldValue) => onChange({ ...value, [field]: fieldValue });

	const toggleContract = (contract) =>
		update(
			"contractTypes",
			value.contractTypes.includes(contract)
				? value.contractTypes.filter((type) => type !== contract)
				: [...value.contractTypes, contract],
		);

	return (
		<div className="space-y-5">
			<div>
				<label htmlFor="soughtPosition" className="mb-2 block text-sm font-medium text-heading">
					Poste / profil recherché
				</label>
				<input
					id="soughtPosition"
					value={value.soughtPosition}
					onChange={(event) => update("soughtPosition", event.target.value)}
					required
					maxLength={150}
					placeholder="Ex : Développeur·se React confirmé·e"
					className={inputClass}
				/>
			</div>

			<fieldset>
				<legend className="mb-2 block text-sm font-medium text-heading">
					Types de contrat <span className="text-xs text-body">(plusieurs choix possibles)</span>
				</legend>
				<div className="flex flex-wrap gap-2">
					{CONTRACT_TYPES.map((contract) => {
						const selected = value.contractTypes.includes(contract.value);
						return (
							<label key={contract.value} className={chipClass(selected)}>
								<input
									type="checkbox"
									checked={selected}
									onChange={() => toggleContract(contract.value)}
									className="sr-only"
								/>
								{contract.label}
							</label>
						);
					})}
				</div>
			</fieldset>

			<div className="grid grid-cols-1 gap-5 sm:grid-cols-2">
				<div>
					<label htmlFor="location" className="mb-2 block text-sm font-medium text-heading">
						Localisation <span className="text-xs text-body">(optionnel)</span>
					</label>
					<input
						id="location"
						value={value.location}
						onChange={(event) => update("location", event.target.value)}
						maxLength={100}
						placeholder="Ex : Bruxelles"
						className={inputClass}
					/>
				</div>

				<div>
					<label htmlFor="remotePolicy" className="mb-2 block text-sm font-medium text-heading">
						Télétravail
					</label>
					<select
						id="remotePolicy"
						value={value.remotePolicy}
						onChange={(event) => update("remotePolicy", event.target.value)}
						className={inputClass}>
						<option value="">Non précisé</option>
						{REMOTE_POLICIES.map((policy) => (
							<option key={policy.value} value={policy.value}>
								{policy.label}
							</option>
						))}
					</select>
				</div>
			</div>

			<div>
				<label htmlFor="sector" className="mb-2 block text-sm font-medium text-heading">
					Secteur d’activité
				</label>
				<select
					id="sector"
					value={value.sector}
					onChange={(event) => update("sector", event.target.value)}
					className={inputClass}>
					<option value="">Non précisé</option>
					{SECTORS.map((sector) => (
						<option key={sector.value} value={sector.value}>
							{sector.label}
						</option>
					))}
				</select>
			</div>
		</div>
	);
};

/** Badges contrat / télétravail / secteur / lieu, pour les cartes et fiches. */
export const RecruiterNeedsBadges = ({ needs }) => {
	if (!needs) return null;

	const badges = [
		...(needs.contractTypes ?? []).map((type) => labelOf(CONTRACT_TYPES, type)),
		needs.remotePolicy && labelOf(REMOTE_POLICIES, needs.remotePolicy),
		needs.location && `📍 ${needs.location}`,
		needs.sector && labelOf(SECTORS, needs.sector),
	].filter(Boolean);

	if (badges.length === 0) return null;

	return (
		<ul className="flex flex-wrap gap-2">
			{badges.map((badge) => (
				<li key={badge} className="rounded-full border border-electric-300/30 bg-electric-500/10 px-3 py-1 text-xs text-heading">
					{badge}
				</li>
			))}
		</ul>
	);
};
