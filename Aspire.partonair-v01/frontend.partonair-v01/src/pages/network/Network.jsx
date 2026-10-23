import { useCallback, useContext, useEffect, useRef, useState } from "react";
import { useNavigate, useSearchParams } from "react-router-dom";
import AuthContext from "../../contexts/auth/Auth-context";
import NotificationsContext from "../../contexts/notifications/Notifications-context";
import RealtimeContext from "../../contexts/realtime/Realtime-context";
import ProfileAvatar from "../../components/ui/Profile-avatar";
import { contactApi } from "../../services/api";

const TABS = [
	{ value: "friends", label: "Amis" },
	{ value: "incoming", label: "Demandes reçues" },
	{ value: "outgoing", label: "Demandes envoyées" },
	{ value: "blocked", label: "Bloqués" },
];

const EMPTY_LISTS = { friends: [], incoming: [], outgoing: [], blocked: [] };

const Network = () => {
	const { auth } = useContext(AuthContext);
	const { refreshIncoming } = useContext(NotificationsContext);
	const { subscribe } = useContext(RealtimeContext);
	const navigate = useNavigate();
	const [searchParams] = useSearchParams();

	const requestedTab = searchParams.get("tab");
	const [activeTab, setActiveTab] = useState(TABS.some((tab) => tab.value === requestedTab) ? requestedTab : "friends");
	// Tant que l'utilisateur n'a pas choisi d'onglet, on ouvre « Demandes reçues » s'il y en a.
	const tabChosenRef = useRef(Boolean(requestedTab));
	const [lists, setLists] = useState(EMPTY_LISTS);
	const [loading, setLoading] = useState(true);
	const [error, setError] = useState(null);
	const [actionState, setActionState] = useState({});

	useEffect(() => {
		if (!auth) {
			navigate("/");
		}
	}, [auth, navigate]);

	useEffect(() => {
		if (!auth) return;

		let cancelled = false;
		Promise.resolve().then(() => {
			if (!cancelled) {
				setLoading(true);
				setError(null);
			}
		});

		Promise.all([
			contactApi.getFriends(),
			contactApi.getIncomingRequests(),
			contactApi.getOutgoingRequests(),
			contactApi.getBlocked(),
		])
			.then(([friendsRes, incomingRes, outgoingRes, blockedRes]) => {
				if (cancelled) return;
				if (!tabChosenRef.current && (incomingRes?.contacts?.length ?? 0) > 0) setActiveTab("incoming");
				setLists({
					friends: friendsRes?.contacts ?? [],
					incoming: incomingRes?.contacts ?? [],
					outgoing: outgoingRes?.contacts ?? [],
					blocked: blockedRes?.contacts ?? [],
				});
			})
			.catch(() => {
				if (!cancelled) setError("Impossible de charger ton réseau. Réessaie.");
			})
			.finally(() => {
				if (!cancelled) setLoading(false);
			});

		return () => {
			cancelled = true;
		};
	}, [auth]);

	// Lien « Voir la demande » d'un toast alors qu'on est déjà sur /network.
	useEffect(() => {
		if (!TABS.some((tab) => tab.value === requestedTab)) return;
		tabChosenRef.current = true;
		Promise.resolve().then(() => setActiveTab(requestedTab));
	}, [requestedTab]);

	// Nouvelle demande reçue en direct : ajout immédiat dans « Demandes reçues ».
	useEffect(() => {
		if (!auth?.userId) return;

		return subscribe("FriendRequestReceived", (request) => {
			setLists((prev) =>
				prev.incoming.some((item) => item.contactId === request.contactId)
					? prev
					: { ...prev, incoming: [request, ...prev.incoming] },
			);
		});
	}, [auth?.userId, subscribe]);

	const selectTab = (tab) => {
		tabChosenRef.current = true;
		setActiveTab(tab);
	};

	const reload = useCallback(() => {
		if (!auth) return;
		contactApi
			.getFriends()
			.then((res) => setLists((prev) => ({ ...prev, friends: res?.contacts ?? [] })))
			.catch(() => {});
		contactApi
			.getIncomingRequests()
			.then((res) => setLists((prev) => ({ ...prev, incoming: res?.contacts ?? [] })))
			.catch(() => {});
		contactApi
			.getOutgoingRequests()
			.then((res) => setLists((prev) => ({ ...prev, outgoing: res?.contacts ?? [] })))
			.catch(() => {});
		contactApi
			.getBlocked()
			.then((res) => setLists((prev) => ({ ...prev, blocked: res?.contacts ?? [] })))
			.catch(() => {});
	}, [auth]);

	// Blocage / déblocage (par moi dans un autre onglet, ou par l'autre utilisateur).
	useEffect(() => {
		if (!auth?.userId) return;
		return subscribe("RelationshipChanged", reload);
	}, [auth?.userId, subscribe, reload]);

	const runAction = async (key, action) => {
		setActionState((prev) => ({ ...prev, [key]: "pending" }));
		try {
			await action();
			reload();
			refreshIncoming();
			setActionState((prev) => ({ ...prev, [key]: "done" }));
		} catch {
			setActionState((prev) => ({ ...prev, [key]: "error" }));
		}
	};

	if (!auth) return null;

	const items = lists[activeTab];

	return (
		<main className="min-h-screen px-4 pb-16 pt-28 text-left">
			<div className="mx-auto w-full max-w-4xl">
				<header className="mx-auto mb-8 max-w-2xl text-center">
					<p className="mb-3 text-xs font-semibold uppercase tracking-[0.2em] text-electric-300">Partonair · réseau</p>
					<h1 className="text-balance text-3xl font-bold leading-tight text-heading sm:text-4xl">Ton réseau</h1>
					<p className="mt-3 text-base leading-relaxed text-body">Gère tes amis, tes demandes et les profils bloqués.</p>
				</header>

				<section className="glass-panel mb-8 rounded-2xl p-4 sm:p-5" aria-label="Filtres du réseau">
					<div className="grid grid-cols-2 gap-2 sm:grid-cols-4" role="tablist" aria-label="Catégorie de contacts">
						{TABS.map((tab) => {
							const selected = activeTab === tab.value;
							const count = lists[tab.value].length;
							const needsAttention = tab.value === "incoming" && count > 0;
							return (
								<button
									key={tab.value}
									type="button"
									role="tab"
									aria-selected={selected}
									onClick={() => selectTab(tab.value)}
									className={`relative rounded-xl px-3 py-3 text-sm font-semibold transition focus-visible:outline-none focus-visible:ring-2 focus-visible:ring-electric-300 ${
										selected
											? "border border-electric-300/40 bg-electric-500/20 text-white shadow-glow"
											: needsAttention
												? "border border-red-400/40 bg-red-500/10 text-heading hover:bg-red-500/15"
												: "border border-white/10 bg-white/[0.03] text-body hover:border-white/20 hover:bg-white/[0.07] hover:text-heading"
									}`}>
									{needsAttention && (
										<span className="absolute -right-1 -top-1 flex h-3 w-3" aria-hidden="true">
											<span className="absolute inline-flex h-full w-full animate-ping rounded-full bg-red-400 opacity-75" />
											<span className="relative inline-flex h-3 w-3 rounded-full bg-red-500" />
										</span>
									)}
									{tab.label}{" "}
									<span className={`text-xs ${needsAttention ? "font-bold text-red-300" : "text-body"}`}>({count})</span>
								</button>
							);
						})}
					</div>
				</section>

				{loading && <p className="py-16 text-center text-body" role="status">Chargement…</p>}
				{!loading && error && <p className="glass-panel rounded-2xl p-8 text-center text-red-300" role="alert">{error}</p>}

				{!loading && !error && items.length === 0 && (
					<div className="glass-panel rounded-2xl px-6 py-12 text-center">
						<h2 className="text-lg font-semibold text-heading">Rien ici pour le moment</h2>
						<p className="mt-2 text-sm text-body">
							{activeTab === "friends" && "Ajoute des profils depuis l’annuaire pour commencer."}
							{activeTab === "incoming" && "Aucune demande en attente."}
							{activeTab === "outgoing" && "Tu n’as envoyé aucune demande en attente."}
							{activeTab === "blocked" && "Tu n’as bloqué personne."}
						</p>
					</div>
				)}

				{!loading && !error && items.length > 0 && (
					<ul className="space-y-3" aria-label={TABS.find((tab) => tab.value === activeTab)?.label}>
						{items.map((item) => {
							const key = `${activeTab}-${item.contactId}`;
							const state = actionState[key];
							return (
								<li key={item.contactId} className="glass-panel flex flex-wrap items-center gap-4 rounded-2xl p-4">
									<ProfileAvatar name={item.userName} seed={item.avatarSeed} className="h-12 w-12" />
									<div className="min-w-0 flex-1">
										<p className="truncate font-semibold text-heading">{item.userName}</p>
										<p className="text-xs text-body">{item.role === "Company" ? "Recruteur / Entreprise" : "Chercheur d’emploi"}</p>
									</div>

									<div className="flex shrink-0 flex-wrap items-center gap-2">
										{item.profileId && (
											<button
												type="button"
												onClick={() => navigate(`/profiles/${item.profileId}`)}
												className="rounded-lg border border-electric-300/30 px-3 py-1.5 text-xs font-medium text-electric-300 transition hover:bg-electric-500/15">
												Voir le profil
											</button>
										)}
										{activeTab === "friends" && (
											<>
												<button
													type="button"
													onClick={() => navigate(`/messages/${item.userId}`)}
													className="rounded-lg bg-electric-500 px-3 py-1.5 text-xs font-semibold text-white transition hover:bg-electric-400">
													Message
												</button>
												<button
													type="button"
													disabled={state === "pending"}
													onClick={() => runAction(key, () => contactApi.removeFriend(item.contactId))}
													className="rounded-lg border border-white/10 px-3 py-1.5 text-xs font-medium text-body transition hover:border-white/20 hover:text-heading disabled:cursor-not-allowed disabled:opacity-60">
													Retirer
												</button>
												<button
													type="button"
													disabled={state === "pending"}
													onClick={() => runAction(key, () => contactApi.block(item.userId))}
													className="rounded-lg border border-red-400/30 px-3 py-1.5 text-xs font-medium text-red-300 transition hover:bg-red-500/10 disabled:cursor-not-allowed disabled:opacity-60">
													Bloquer
												</button>
											</>
										)}

										{activeTab === "incoming" && (
											<>
												<button
													type="button"
													disabled={state === "pending"}
													onClick={() => runAction(key, () => contactApi.acceptRequest(item.contactId))}
													className="rounded-lg bg-electric-500 px-3 py-1.5 text-xs font-semibold text-white transition hover:bg-electric-400 disabled:cursor-not-allowed disabled:opacity-60">
													Accepter
												</button>
												<button
													type="button"
													disabled={state === "pending"}
													onClick={() => runAction(key, () => contactApi.refuseRequest(item.contactId))}
													className="rounded-lg border border-white/10 px-3 py-1.5 text-xs font-medium text-body transition hover:border-white/20 hover:text-heading disabled:cursor-not-allowed disabled:opacity-60">
													Refuser
												</button>
											</>
										)}

										{activeTab === "outgoing" && (
											<span className="rounded-full border border-white/10 px-3 py-1 text-xs text-body">En attente</span>
										)}

										{activeTab === "blocked" && (
											<button
												type="button"
												disabled={state === "pending"}
												onClick={() => runAction(key, () => contactApi.unblock(item.userId))}
												className="rounded-lg border border-white/10 px-3 py-1.5 text-xs font-medium text-body transition hover:border-white/20 hover:text-heading disabled:cursor-not-allowed disabled:opacity-60">
												Débloquer
											</button>
										)}
									</div>

									{state === "error" && <p className="w-full text-right text-xs text-red-300">Action impossible, réessaie.</p>}
								</li>
							);
						})}
					</ul>
				)}
			</div>
		</main>
	);
};

export default Network;
