const firstNameFrom = (displayName = "") => displayName.trim().split(/\s+/)[0] || "";

// seed : identifiant DiceBear stocké sur le profil ; à défaut on retombe sur le prénom.
const ProfileAvatar = ({ name, seed: avatarSeed, className = "h-14 w-14" }) => {
    const firstName = firstNameFrom(name);
    const seed = avatarSeed || firstName || "Partonair";
    const avatarUrl = `https://api.dicebear.com/10.x/pixel-art/svg?seed=${encodeURIComponent(seed)}`;

    return (
        <span
            className={`relative inline-flex shrink-0 items-center justify-center overflow-hidden rounded-full border border-white/15 bg-electric-600/20 text-lg font-semibold text-heading ${className}`}
            aria-hidden="true">
            <span>{firstName.charAt(0).toUpperCase() || "?"}</span>
            <img
                key={seed}
                src={avatarUrl}
                alt=""
                loading="lazy"
                referrerPolicy="no-referrer"
                onError={(event) => {
                    event.currentTarget.hidden = true;
                }}
                className="absolute inset-0 h-full w-full object-cover"
            />
        </span>
    );
};

export default ProfileAvatar;
