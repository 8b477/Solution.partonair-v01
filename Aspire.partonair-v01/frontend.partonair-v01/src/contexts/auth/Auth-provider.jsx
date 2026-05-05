import { useState } from "react";
import AuthContext from "./Auth-context";

const AuthProvider = ({ children }) => {
	const [auth, setAuth] = useState(() => {
		const stored = localStorage.getItem("auth");
		return stored ? JSON.parse(stored) : null;
	});

	const login = ({ token, userId, userName, profileId, role }) => {
		const data = { token, userId, userName, profileId, role };
		setAuth(data);
		localStorage.setItem("auth", JSON.stringify(data));
	};

	const updateProfileId = (profileId) => {
		setAuth((prev) => {
			const updated = { ...prev, profileId };
			localStorage.setItem("auth", JSON.stringify(updated));
			return updated;
		});
	};

	const logout = () => {
		setAuth(null);
		localStorage.removeItem("auth");
	};

	return (
		<AuthContext.Provider value={{ auth, login, logout, updateProfileId }}>
			{children}
		</AuthContext.Provider>
	);
};

export default AuthProvider;
