import { useCallback, useContext, useEffect, useRef, useState } from "react";
import AuthContext from "../auth/Auth-context";
import RealtimeContext from "../realtime/Realtime-context";
import NotificationsContext from "./Notifications-context";
import Toasts from "../../components/ui/Toasts";
import { contactApi, messageApi } from "../../services/api";

const TOAST_DURATION_MS = 6000;
const PREVIEW_LENGTH = 80;

const preview = (content) => (content.length > PREVIEW_LENGTH ? `${content.slice(0, PREVIEW_LENGTH)}…` : content);

// Compteurs « non consultés » (demandes d'ami en attente, messages non lus) + toasts temps réel.
const NotificationsProvider = ({ children }) => {
	const { auth } = useContext(AuthContext);
	const { subscribe } = useContext(RealtimeContext);

	const [incomingCount, setIncomingCount] = useState(0);
	// userId de l'expéditeur -> nombre de messages non lus
	const [unreadBySender, setUnreadBySender] = useState({});
	const [toasts, setToasts] = useState([]);
	const timersRef = useRef(new Map());
	const nextToastIdRef = useRef(0);
	// Conversation affichée à l'écran : ses messages sont lus directement, sans toast ni badge.
	const activeConversationRef = useRef(null);

	const dismissToast = useCallback((id) => {
		clearTimeout(timersRef.current.get(id));
		timersRef.current.delete(id);
		setToasts((prev) => prev.filter((toast) => toast.id !== id));
	}, []);

	const addToast = useCallback(
		(toast) => {
			const id = ++nextToastIdRef.current;
			setToasts((prev) => [...prev, { ...toast, id }]);
			timersRef.current.set(id, setTimeout(() => dismissToast(id), TOAST_DURATION_MS));
		},
		[dismissToast],
	);

	const refreshIncoming = useCallback(() => {
		contactApi
			.getIncomingRequests()
			.then((res) => setIncomingCount(res?.contacts?.length ?? 0))
			.catch(() => {});
	}, []);

	const refreshUnread = useCallback(() => {
		messageApi
			.getUnread()
			.then((res) =>
				setUnreadBySender(
					Object.fromEntries((res?.conversations ?? []).map((conversation) => [conversation.userId, conversation.count])),
				),
			)
			.catch(() => {});
	}, []);

	const markConversationRead = useCallback((userId) => {
		if (!userId) return;
		setUnreadBySender((prev) => {
			if (!prev[userId]) return prev;
			const next = { ...prev };
			delete next[userId];
			return next;
		});
		messageApi.markRead(userId).catch(() => {});
	}, []);

	const setActiveConversation = useCallback(
		(userId) => {
			activeConversationRef.current = userId ?? null;
			if (userId) markConversationRead(userId);
		},
		[markConversationRead],
	);

	useEffect(() => {
		if (!auth?.userId) return;

		// Couvre ce qui a été reçu pendant qu'on était hors ligne.
		refreshIncoming();
		refreshUnread();

		const unsubscribeRequest = subscribe("FriendRequestReceived", (request) => {
			refreshIncoming();
			addToast({
				title: "Nouvelle demande d’ami",
				message: `${request.userName} veut t’ajouter à son réseau.`,
				avatar: { name: request.userName, seed: request.avatarSeed },
				action: { label: "Voir la demande", to: "/network?tab=incoming" },
			});
		});

		const unsubscribeMessage = subscribe("MessageReceived", (message) => {
			if (message.receiverId !== auth.userId) return;

			if (activeConversationRef.current === message.senderId) {
				markConversationRead(message.senderId);
				return;
			}

			setUnreadBySender((prev) => ({ ...prev, [message.senderId]: (prev[message.senderId] ?? 0) + 1 }));
			addToast({
				title: `Nouveau message de ${message.senderName || "un contact"}`,
				message: preview(message.content),
				avatar: { name: message.senderName, seed: message.senderAvatarSeed },
				action: { label: "Répondre", to: `/messages/${message.senderId}` },
			});
		});

		const unsubscribeEvaluation = subscribe("EvaluationReceived", ({ evaluation, isNew }) => {
			addToast({
				title: isNew
					? `${evaluation.senderName} t’a laissé un avis`
					: `${evaluation.senderName} a modifié son avis`,
				message: `${"★".repeat(evaluation.value)}${"☆".repeat(5 - evaluation.value)} · ${preview(evaluation.commentary)}`,
				avatar: { name: evaluation.senderName, seed: evaluation.senderAvatarSeed },
				action: { label: "Voir mes avis", to: "/profile" },
			});
		});

		const unsubscribeReconnected = subscribe("__reconnected", () => {
			refreshIncoming();
			refreshUnread();
		});

		return () => {
			unsubscribeRequest();
			unsubscribeMessage();
			unsubscribeEvaluation();
			unsubscribeReconnected();
		};
	}, [auth?.userId, subscribe, refreshIncoming, refreshUnread, markConversationRead, addToast]);

	useEffect(() => {
		const timers = timersRef.current;
		return () => timers.forEach((timer) => clearTimeout(timer));
	}, []);

	const unreadTotal = Object.values(unreadBySender).reduce((sum, count) => sum + count, 0);

	return (
		<NotificationsContext.Provider
			value={{
				incomingCount: auth ? incomingCount : 0,
				unreadTotal: auth ? unreadTotal : 0,
				unreadBySender: auth ? unreadBySender : {},
				refreshIncoming,
				setActiveConversation,
				addToast,
			}}>
			{children}
			{auth && <Toasts toasts={toasts} onDismiss={dismissToast} />}
		</NotificationsContext.Provider>
	);
};

export default NotificationsProvider;
