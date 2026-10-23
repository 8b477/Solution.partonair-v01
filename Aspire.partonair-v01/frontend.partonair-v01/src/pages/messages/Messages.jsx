import { useContext, useEffect, useRef, useState } from "react";
import { useNavigate, useParams } from "react-router-dom";
import AuthContext from "../../contexts/auth/Auth-context";
import RealtimeContext from "../../contexts/realtime/Realtime-context";
import NotificationsContext from "../../contexts/notifications/Notifications-context";
import ProfileAvatar from "../../components/ui/Profile-avatar";
import { contactApi, messageApi } from "../../services/api";

// Au-delà de cet écart, un nouveau message du même auteur ouvre un nouveau groupe (nouvel en-tête).
const GROUP_GAP_MS = 5 * 60 * 1000;

const BLOCKED_STATUSES = ["BlockedByMe", "BlockedByOther"];

// Message renvoyé par le backend (HubException côté SignalR, ProblemDetails côté REST).
const serverReason = (err) => err?.serverMessage || err?.message?.split("HubException: ")[1] || null;

const formatTime = (date) => date.toLocaleTimeString("fr-FR", { hour: "2-digit", minute: "2-digit" });

const isSameDay = (a, b) => a.toDateString() === b.toDateString();

const formatDay = (date) => {
	const today = new Date();
	const yesterday = new Date();
	yesterday.setDate(today.getDate() - 1);

	if (isSameDay(date, today)) return "Aujourd’hui";
	if (isSameDay(date, yesterday)) return "Hier";
	return date.toLocaleDateString("fr-FR", {
		day: "numeric",
		month: "short",
		...(date.getFullYear() !== today.getFullYear() ? { year: "numeric" } : {}),
	});
};

const Messages = () => {
	const { auth } = useContext(AuthContext);
	const { connectionState, subscribe, invoke, isConnected } = useContext(RealtimeContext);
	const { unreadBySender, setActiveConversation } = useContext(NotificationsContext);
	const navigate = useNavigate();
	const { friendId } = useParams();

	const [friends, setFriends] = useState([]);
	const [friendsLoading, setFriendsLoading] = useState(true);
	const [friendsError, setFriendsError] = useState(null);

	const [history, setHistory] = useState([]);
	const [historyLoading, setHistoryLoading] = useState(false);
	const [historyError, setHistoryError] = useState(null);

	// Lecture seule si un blocage existe : blockedBy = "me" | "other".
	const [conversationAccess, setConversationAccess] = useState({ canSend: true, blockedBy: null });

	const [draft, setDraft] = useState("");
	const [sendError, setSendError] = useState(null);
	const [unblocking, setUnblocking] = useState(false);
	// Incrémenté après une reconnexion SignalR pour recharger les messages manqués pendant la coupure.
	const [historyVersion, setHistoryVersion] = useState(0);
	// Incrémenté quand un blocage / déblocage change la liste des contacts.
	const [contactsVersion, setContactsVersion] = useState(0);

	const bottomRef = useRef(null);
	// Le handler temps réel est enregistré une seule fois : il lit la conversation active via cette ref,
	// sinon il garderait la valeur de friendId du premier rendu et rejetterait tous les messages.
	const friendIdRef = useRef(friendId);
	const loadedFriendIdRef = useRef(null);

	useEffect(() => {
		friendIdRef.current = friendId;
	}, [friendId]);

	// Conversation ouverte = messages lus (et plus de toast pour cet ami tant qu'elle reste affichée).
	useEffect(() => {
		if (!auth?.userId) return;
		setActiveConversation(friendId);
		return () => setActiveConversation(null);
	}, [auth?.userId, friendId, setActiveConversation]);

	useEffect(() => {
		if (!auth) {
			navigate("/");
		}
	}, [auth, navigate]);

	// Amis + contacts bloqués (dans les deux sens) : une conversation bloquée reste lisible.
	useEffect(() => {
		if (!auth) return;

		let cancelled = false;
		contactApi
			.getConversationContacts()
			.then((data) => {
				if (!cancelled) setFriends(data?.contacts ?? []);
			})
			.catch(() => {
				if (!cancelled) setFriendsError("Impossible de charger tes amis.");
			})
			.finally(() => {
				if (!cancelled) setFriendsLoading(false);
			});

		return () => {
			cancelled = true;
		};
	}, [auth, contactsVersion]);

	// Historique REST de la conversation active.
	useEffect(() => {
		if (!auth || !friendId) return;

		let cancelled = false;
		Promise.resolve().then(() => {
			if (!cancelled) {
				// Changement de conversation : on n'affiche pas l'ancienne pendant le chargement.
				if (loadedFriendIdRef.current !== friendId) {
					setHistory([]);
					setHistoryLoading(true);
				}
				setHistoryError(null);
			}
		});

		messageApi
			.getConversation(friendId)
			.then((data) => {
				if (cancelled) return;
				loadedFriendIdRef.current = friendId;
				setHistory(data?.messages ?? []);
				setConversationAccess({ canSend: data?.canSend ?? true, blockedBy: data?.blockedBy ?? null });
			})
			.catch((err) => {
				if (cancelled) return;
				loadedFriendIdRef.current = null;
				setHistory([]);
				setConversationAccess({ canSend: false, blockedBy: null });
				setHistoryError(
					err.status === 409
						? "Vous devez être amis pour voir cette conversation."
						: "Impossible de charger la conversation."
				);
			})
			.finally(() => {
				if (!cancelled) setHistoryLoading(false);
			});

		return () => {
			cancelled = true;
		};
	}, [auth, friendId, historyVersion]);

	// Messages temps réel : la connexion est portée par RealtimeProvider, on ne fait que s'abonner.
	useEffect(() => {
		if (!auth?.userId) return;

		const unsubscribeMessage = subscribe("MessageReceived", (message) => {
			const activeFriendId = friendIdRef.current;
			const belongsToConversation =
				activeFriendId &&
				((message.senderId === auth.userId && message.receiverId === activeFriendId) ||
					(message.senderId === activeFriendId && message.receiverId === auth.userId));

			if (!belongsToConversation) return;
			setHistory((prev) => (prev.some((existing) => existing.id === message.id) ? prev : [...prev, message]));
		});
		const unsubscribeReconnected = subscribe("__reconnected", () => setHistoryVersion((version) => version + 1));
		// Blocage / déblocage par l'un ou l'autre : on rafraîchit la liste et, si besoin, la conversation ouverte.
		const unsubscribeRelationship = subscribe("RelationshipChanged", ({ userId }) => {
			setContactsVersion((version) => version + 1);
			if (userId === friendIdRef.current) setHistoryVersion((version) => version + 1);
		});

		return () => {
			unsubscribeMessage();
			unsubscribeReconnected();
			unsubscribeRelationship();
		};
	}, [auth?.userId, subscribe]);

	useEffect(() => {
		bottomRef.current?.scrollIntoView({ behavior: "smooth" });
	}, [history]);

	const sendMessage = async (event) => {
		event.preventDefault();
		const content = draft.trim();
		if (!content || !friendId || !conversationAccess.canSend) return;

		setSendError(null);

		try {
			if (isConnected()) {
				await invoke("SendMessage", friendId, content);
			} else {
				const data = await messageApi.send(friendId, content);
				if (data?.message) {
					setHistory((prev) => (prev.some((m) => m.id === data.message.id) ? prev : [...prev, data.message]));
				}
			}
			setDraft("");
		} catch (err) {
			setSendError(serverReason(err) ?? "Le message n’a pas pu être envoyé. Vérifie que vous êtes toujours amis.");
			// L'envoi a pu échouer à cause d'un blocage tout juste posé : on resynchronise l'accès.
			setHistoryVersion((version) => version + 1);
		}
	};

	const unblockFriend = async () => {
		setUnblocking(true);
		setSendError(null);
		try {
			await contactApi.unblock(friendId);
			setContactsVersion((version) => version + 1);
			setHistoryVersion((version) => version + 1);
		} catch {
			setSendError("Impossible de débloquer ce contact. Réessaie.");
		} finally {
			setUnblocking(false);
		}
	};

	if (!auth) return null;

	const activeFriend = friends.find((friend) => friend.userId === friendId);
	const activeName = activeFriend?.userName ?? "Ce contact";

	return (
		<main className="min-h-screen px-4 pb-16 pt-28 text-left">
			<div className="mx-auto w-full max-w-5xl">
				<header className="mb-8 text-center">
					<p className="mb-3 text-xs font-semibold uppercase tracking-[0.2em] text-electric-300">Partonair · messagerie</p>
					<h1 className="text-balance text-3xl font-bold leading-tight text-heading sm:text-4xl">Messages privés</h1>
					<p className="mt-3 text-base leading-relaxed text-body">Discute en temps réel avec tes contacts acceptés.</p>
				</header>

				<div className="grid grid-cols-1 gap-5 md:grid-cols-[18rem_1fr]">
					<section className="glass-panel rounded-2xl p-4" aria-label="Liste des amis">
						<h2 className="mb-3 text-sm font-semibold uppercase tracking-wider text-body">Conversations</h2>
						{friendsLoading && <p className="text-sm text-body">Chargement…</p>}
						{!friendsLoading && friendsError && <p className="text-sm text-red-300">{friendsError}</p>}
						{!friendsLoading && !friendsError && friends.length === 0 && (
							<p className="text-sm text-body">Ajoute des amis pour commencer à discuter.</p>
						)}
						<ul className="space-y-1">
							{friends.map((friend) => {
								const isBlocked = BLOCKED_STATUSES.includes(friend.status);
								return (
									<li key={friend.contactId}>
										<button
											type="button"
											onClick={() => navigate(`/messages/${friend.userId}`)}
											className={`flex w-full items-center gap-3 rounded-xl px-3 py-2 text-left transition ${
												friend.userId === friendId
													? "border border-electric-300/40 bg-electric-500/20 text-white"
													: "border border-transparent hover:bg-white/[0.06]"
											} ${isBlocked ? "opacity-60" : ""}`}>
											<ProfileAvatar name={friend.userName} seed={friend.avatarSeed} className={`h-9 w-9 text-sm ${isBlocked ? "grayscale" : ""}`} />
											<span className="min-w-0 flex-1">
												<span className="block truncate text-sm font-medium text-heading">{friend.userName}</span>
												{isBlocked && (
													<span className="block text-[0.7rem] text-red-300">
														{friend.status === "BlockedByMe" ? "🔒 Bloqué par toi" : "🔒 T’a bloqué"}
													</span>
												)}
											</span>
											{friend.userId !== friendId && unreadBySender[friend.userId] > 0 && (
												<span
													className="inline-flex h-5 min-w-5 items-center justify-center rounded-full bg-red-500 px-1.5 text-[0.65rem] font-bold leading-none text-white"
													aria-label={`${unreadBySender[friend.userId]} message${unreadBySender[friend.userId] > 1 ? "s" : ""} non lu${unreadBySender[friend.userId] > 1 ? "s" : ""}`}>
													{unreadBySender[friend.userId] > 9 ? "9+" : unreadBySender[friend.userId]}
												</span>
											)}
										</button>
									</li>
								);
							})}
						</ul>
					</section>

					<section className="glass-panel flex min-h-[32rem] flex-col rounded-2xl p-4 sm:p-6" aria-label="Conversation">
						{!friendId && (
							<div className="flex flex-1 items-center justify-center text-center text-body">
								<p>Choisis un ami dans la liste pour démarrer une conversation.</p>
							</div>
						)}

						{friendId && (
							<>
								<div className="mb-4 flex items-center justify-between gap-3 border-b border-white/10 pb-4">
									<div className="flex items-center gap-3">
										<ProfileAvatar name={activeFriend?.userName ?? "?"} seed={activeFriend?.avatarSeed} className="h-10 w-10" />
										<div>
											<p className="font-semibold text-heading">{activeFriend?.userName ?? "Conversation"}</p>
											<p className="text-xs text-body">
												{connectionState === "connected" && "Connecté en temps réel"}
												{connectionState === "connecting" && "Connexion…"}
												{connectionState === "reconnecting" && "Reconnexion…"}
												{connectionState === "disconnected" && "Hors ligne — les messages passent par le réseau classique"}
											</p>
										</div>
									</div>
								</div>

								<div className="flex-1 overflow-y-auto pr-1">
									{historyLoading && <p className="py-10 text-center text-body" role="status">Chargement…</p>}
									{!historyLoading && historyError && <p className="py-10 text-center text-red-300" role="alert">{historyError}</p>}
									{!historyLoading && !historyError && history.length === 0 && (
										<p className="py-10 text-center text-body">Aucun message pour l’instant. Dis bonjour 👋</p>
									)}
									{!historyLoading &&
										!historyError &&
										history.map((message, index) => {
											const isMine = message.senderId === auth.userId;
											const sentAt = new Date(message.sentAt);
											const previous = history[index - 1];
											const previousSentAt = previous && new Date(previous.sentAt);

											const startsDay = !previous || !isSameDay(previousSentAt, sentAt);
											const startsGroup =
												startsDay || previous.senderId !== message.senderId || sentAt - previousSentAt > GROUP_GAP_MS;
											const authorName = isMine ? "Moi" : message.senderName || activeFriend?.userName || "Contact";

											return (
												<div key={message.id}>
													{startsDay && (
														<div className="my-4 flex items-center gap-3" role="separator">
															<span className="h-px flex-1 bg-white/10" />
															<span className="text-[0.7rem] font-medium uppercase tracking-wider text-body">{formatDay(sentAt)}</span>
															<span className="h-px flex-1 bg-white/10" />
														</div>
													)}

													<div className={`flex items-end gap-2 ${isMine ? "justify-end" : "justify-start"} ${startsGroup ? "mt-4" : "mt-1"}`}>
														{!isMine &&
															(startsGroup ? (
																<ProfileAvatar
																	name={authorName}
																	seed={message.senderAvatarSeed ?? activeFriend?.avatarSeed}
																	className="mb-0.5 h-7 w-7 self-start text-xs"
																/>
															) : (
																<span className="w-7 shrink-0" aria-hidden="true" />
															))}

														<div className={`flex max-w-[75%] flex-col ${isMine ? "items-end" : "items-start"}`}>
															{startsGroup && (
																<p className={`mb-1 px-1 text-[0.7rem] ${isMine ? "text-electric-300" : "text-body"}`}>
																	<span className="font-semibold">{authorName}</span>
																	<span className="text-body/70"> · {formatTime(sentAt)}</span>
																</p>
															)}
															<div
																title={formatTime(sentAt)}
																className={`rounded-2xl px-4 py-2 text-sm ${
																	isMine
																		? `bg-electric-500 text-white ${startsGroup ? "rounded-tr-md" : ""}`
																		: `border border-white/10 bg-white/[0.04] text-heading ${startsGroup ? "rounded-tl-md" : ""}`
																}`}>
																<p className="whitespace-pre-line break-words">{message.content}</p>
															</div>
														</div>
													</div>
												</div>
											);
										})}
									<div ref={bottomRef} />
								</div>

								{conversationAccess.blockedBy && (
									<div
										role="status"
										className="mt-4 flex flex-wrap items-center justify-between gap-3 rounded-xl border border-red-400/30 bg-red-500/10 px-4 py-3 text-sm text-red-200">
										<p>
											{conversationAccess.blockedBy === "other"
												? `🔒 ${activeName} t’a bloqué. Tu ne peux plus lui écrire, l’historique reste consultable.`
												: `🔒 Tu as bloqué ${activeName}. Débloque ce contact pour lui écrire à nouveau.`}
										</p>
										{conversationAccess.blockedBy === "me" && (
											<button
												type="button"
												onClick={unblockFriend}
												disabled={unblocking}
												className="rounded-lg border border-white/15 px-3 py-1.5 text-xs font-semibold text-heading transition hover:bg-white/10 disabled:cursor-not-allowed disabled:opacity-60">
												{unblocking ? "Déblocage…" : "Débloquer"}
											</button>
										)}
									</div>
								)}

								<form onSubmit={sendMessage} className="mt-4 flex items-center gap-2 border-t border-white/10 pt-4">
									<label htmlFor="message-draft" className="sr-only">Écrire un message</label>
									<input
										id="message-draft"
										type="text"
										value={draft}
										onChange={(event) => setDraft(event.target.value)}
										maxLength={4000}
										disabled={!conversationAccess.canSend}
										placeholder={conversationAccess.canSend ? "Écris ton message…" : "Conversation en lecture seule"}
										className="block w-full rounded-xl border border-white/10 bg-[#080E1B]/70 px-4 py-2.5 text-sm text-heading placeholder:text-body/70 focus:border-electric-300/60 focus:outline-none focus:ring-2 focus:ring-electric-500/30 disabled:cursor-not-allowed disabled:opacity-50"
									/>
									<button
										type="submit"
										disabled={!draft.trim() || !conversationAccess.canSend}
										className="rounded-xl bg-electric-500 px-4 py-2.5 text-sm font-semibold text-white transition hover:bg-electric-400 disabled:cursor-not-allowed disabled:opacity-60">
										Envoyer
									</button>
								</form>
								{sendError && <p className="mt-2 text-sm text-red-300">{sendError}</p>}
							</>
						)}
					</section>
				</div>
			</div>
		</main>
	);
};

export default Messages;
