// Valeurs alignées sur les enums backend (Domain.partonair_v01.Enums), sérialisées en chaînes.

export const CONTRACT_TYPES = [
	{ value: "CDI", label: "CDI" },
	{ value: "CDD", label: "CDD" },
	{ value: "Freelance", label: "Freelance" },
	{ value: "Stage", label: "Stage" },
	{ value: "Alternance", label: "Alternance" },
];

export const REMOTE_POLICIES = [
	{ value: "OnSite", label: "Sur site" },
	{ value: "Hybrid", label: "Hybride" },
	{ value: "FullRemote", label: "Full remote" },
];

export const SECTORS = [
	{ value: "Tech", label: "Informatique / Tech" },
	{ value: "Sales", label: "Commerce / Vente" },
	{ value: "Marketing", label: "Marketing / Communication" },
	{ value: "Finance", label: "Finance / Comptabilité" },
	{ value: "HumanResources", label: "Ressources humaines" },
	{ value: "Industry", label: "Industrie / Production" },
	{ value: "Health", label: "Santé / Social" },
	{ value: "Construction", label: "BTP / Construction" },
	{ value: "Logistics", label: "Logistique / Transport" },
	{ value: "Other", label: "Autre" },
];

export const EMPTY_RECRUITER_NEEDS = {
	soughtPosition: "",
	contractTypes: [],
	location: "",
	remotePolicy: "",
	sector: "",
};

export const labelOf = (options, value) => options.find((option) => option.value === value)?.label ?? value;

// Formulaire -> payload API (chaînes vides converties en null).
export const toRecruiterNeedsPayload = (needs) => ({
	soughtPosition: needs.soughtPosition.trim() || null,
	contractTypes: needs.contractTypes,
	location: needs.location.trim() || null,
	remotePolicy: needs.remotePolicy || null,
	sector: needs.sector || null,
});

// Réponse API -> formulaire.
export const toRecruiterNeedsForm = (needs) => ({
	soughtPosition: needs?.soughtPosition ?? "",
	contractTypes: needs?.contractTypes ?? [],
	location: needs?.location ?? "",
	remotePolicy: needs?.remotePolicy ?? "",
	sector: needs?.sector ?? "",
});
