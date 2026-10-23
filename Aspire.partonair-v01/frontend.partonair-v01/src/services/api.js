const API_BASE = "/api";

export function getToken() {
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

		// Tente de récupérer le message d'erreur renvoyé par le backend
		try {
			const body = await response.text();
			if (body) {
				try {
					const json = JSON.parse(body);
					error.serverMessage =
						json.message || json.title || (typeof json === "string" ? json : null);
				} catch {
					error.serverMessage = body;
				}
			}
		} catch {
			// pas de corps lisible, on garde le message par défaut
		}

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
	getMe: () => request("/Profile/me"),
	updateMe: (data) =>
		request("/Profile/me", {
			method: "PUT",
			body: JSON.stringify(data),
		}),
	regenerateAvatar: () => request("/Profile/me/avatar/regenerate", { method: "POST" }),
	getById: (id) => request(`/Profile/${id}`),
	getPublicById: (id) => request(`/Profile/public/${id}`),
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
	getFriends: () => request("/Contact/friends"),
	getIncomingRequests: () => request("/Contact/requests/incoming"),
	getOutgoingRequests: () => request("/Contact/requests/outgoing"),
	getBlocked: () => request("/Contact/blocked"),
	getConversationContacts: () => request("/Contact/conversations"),
	getRelationship: (otherUserId) => request(`/Contact/relationship/${encodeURIComponent(otherUserId)}`),
	sendRequest: (receiverId) =>
		request("/Contact", { method: "POST", body: JSON.stringify({ id_Receiver: receiverId }) }),
	acceptRequest: (contactId) =>
		request(`/Contact/AcceptedInvitation?idContact=${encodeURIComponent(contactId)}`, { method: "PATCH" }),
	refuseRequest: (contactId) =>
		request(`/Contact/RefusedInvitation?idContact=${encodeURIComponent(contactId)}`, { method: "PATCH" }),
	removeFriend: (contactId) => request(`/Contact/${encodeURIComponent(contactId)}`, { method: "DELETE" }),
	block: (userId) => request("/Contact/LockContact", { method: "PATCH", body: JSON.stringify({ id_UserToLock: userId }) }),
	unblock: (userId) => request("/Contact/UnlockContact", { method: "PATCH", body: JSON.stringify({ id_UserToUnlock: userId }) }),
};

export const messageApi = {
	getUnread: () => request("/Message/unread"),
	markRead: (otherUserId) => request(`/Message/${encodeURIComponent(otherUserId)}/read`, { method: "POST" }),
	getConversation: (otherUserId, take = 50) =>
		request(`/Message/${encodeURIComponent(otherUserId)}?take=${take}`),
	send: (receiverId, content) =>
		request(`/Message/${encodeURIComponent(receiverId)}`, {
			method: "POST",
			body: JSON.stringify({ content }),
		}),
};

export const evaluationApi = {
	getForUser: (userId) => request(`/Evaluation/user/${encodeURIComponent(userId)}`),
	upsert: (userId, data) =>
		request(`/Evaluation/user/${encodeURIComponent(userId)}`, {
			method: "PUT",
			body: JSON.stringify(data),
		}),
	remove: (userId) => request(`/Evaluation/user/${encodeURIComponent(userId)}`, { method: "DELETE" }),
};
