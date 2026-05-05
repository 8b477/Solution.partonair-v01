const API_BASE = "/api";

function getToken() {
	const stored = localStorage.getItem("auth");
	return stored ? JSON.parse(stored).token : null;
}

async function request(endpoint, options = {}) {
	const url = `${API_BASE}${endpoint}`;
	const token = getToken();

	const config = {
		headers: {
			"Content-Type": "application/json",
			...(token ? { Authorization: `Bearer ${token}` } : {}),
			...options.headers,
		},
		...options,
	};

	const response = await fetch(url, config);

	if (!response.ok) {
		const error = new Error(`${response.status} ${response.statusText}`);
		error.status = response.status;
		throw error;
	}

	if (response.status === 204) return null;

	return response.json();
}

export const signinApi = {
	login: (data) =>
		request("/SigninDefault/login", {
			method: "POST",
			body: JSON.stringify(data),
		}),
};

export const userApi = {
	getById: (id) => request(`/User/${id}`),
	create: (data) =>
		request("/User", {
			method: "POST",
			body: JSON.stringify(data),
		}),
	changeRole: (id, newRole) =>
		request(`/User/${id}/role?newRole=${encodeURIComponent(newRole)}`, {
			method: "PATCH",
		}),
};

export const profileApi = {
	getById: (id) => request(`/Profile/${id}`),
	getByRole: (role) => request(`/Profile?role=${encodeURIComponent(role)}`),
	create: (idUser, data) =>
		request(`/Profile?idUser=${idUser}`, {
			method: "POST",
			body: JSON.stringify(data),
		}),
	update: (id, data) =>
		request(`/Profile/${id}`, {
			method: "PUT",
			body: JSON.stringify(data),
		}),
};

export const contactApi = {
	getAll: () => request("/Contact"),
};

export const evaluationApi = {
	getAll: () => request("/Evaluation"),
};
