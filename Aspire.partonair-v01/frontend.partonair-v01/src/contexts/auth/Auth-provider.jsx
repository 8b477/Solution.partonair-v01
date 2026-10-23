import { useState } from "react";
import AuthContext from "./Auth-context";

const AuthProvider = ({ children }) => {
	const [auth, setAuth] = useState(() => {
		const stored = localStorage.getItem("auth");
		return stored ? JSON.parse(stored) : null;
	});

	const login = ({ token, userId, userName, profileId, role, avatarSeed }) => {
		const data = { token, userId, userName, profileId, role, avatarSeed };
		setAuth(data);
		localStorage.setItem("auth", JSON.stringify(data));
	};

	// Met à jour une partie de la session (profileId, role, userName, avatarSeed...) sans se reconnecter.
	const updateAuth = (changes) => {
		setAuth((prev) => {
			const updated = { ...prev, ...changes };
			localStorage.setItem("auth", JSON.stringify(updated));
			return updated;
		});
	};

	const logout = () => {
		setAuth(null);
		localStorage.removeItem("auth");
	};

	return (
		<AuthContext.Provider value={{ auth, login, logout, updateAuth }}>
			{children}
		</AuthContext.Provider>
	);
};

export default AuthProvider;
