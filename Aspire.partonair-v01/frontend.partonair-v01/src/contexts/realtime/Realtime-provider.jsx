import { useCallback, useContext, useEffect, useRef, useState } from "react";
import * as signalR from "@microsoft/signalr";
import AuthContext from "../auth/Auth-context";
import RealtimeContext from "./Realtime-context";
import { getToken } from "../../services/api";

// Connexion SignalR unique pour toute la session (messagerie + notifications).
// Les pages s'abonnent aux événements via subscribe(), sans gérer la connexion elles-mêmes.
const RealtimeProvider = ({ children }) => {
	const { auth } = useContext(AuthContext);
	const [connectionState, setConnectionState] = useState("connecting");
	const connectionRef = useRef(null);
	// event -> Set(handlers). Un seul dispatcher est enregistré par événement sur la connexion.
	const handlersRef = useRef(new Map());

	const dispatcherFor = useCallback(
		(eventName) =>
			(...args) => {
				handlersRef.current.get(eventName)?.forEach((handler) => handler(...args));
			},
		[],
	);

	useEffect(() => {
		if (!auth?.userId) return;

		const connection = new signalR.HubConnectionBuilder()
			.withUrl("/hubs/realtime", { accessTokenFactory: () => getToken() || "" })
			.withAutomaticReconnect()
			.build();

		handlersRef.current.forEach((_, eventName) => {
			if (eventName !== "__reconnected") connection.on(eventName, dispatcherFor(eventName));
		});

		connection.onreconnecting(() => setConnectionState("reconnecting"));
		connection.onreconnected(() => {
			setConnectionState("connected");
			// Permet aux pages de recharger ce qui a pu être manqué pendant la coupure.
			handlersRef.current.get("__reconnected")?.forEach((handler) => handler());
		});
		connection.onclose(() => setConnectionState("disconnected"));

		connectionRef.current = connection;
		connection
			.start()
			.then(() => setConnectionState("connected"))
			.catch(() => setConnectionState("disconnected"));

		return () => {
			connectionRef.current = null;
			connection.stop();
		};
	}, [auth?.userId, dispatcherFor]);

	const subscribe = useCallback(
		(eventName, handler) => {
			let handlers = handlersRef.current.get(eventName);
			if (!handlers) {
				handlers = new Set();
				handlersRef.current.set(eventName, handlers);
				if (eventName !== "__reconnected") connectionRef.current?.on(eventName, dispatcherFor(eventName));
			}
			handlers.add(handler);

			return () => handlers.delete(handler);
		},
		[dispatcherFor],
	);

	const invoke = useCallback(async (methodName, ...args) => {
		const connection = connectionRef.current;
		if (!connection || connection.state !== signalR.HubConnectionState.Connected) {
			throw new Error("Connexion temps réel indisponible.");
		}
		return connection.invoke(methodName, ...args);
	}, []);

	const isConnected = useCallback(
		() => connectionRef.current?.state === signalR.HubConnectionState.Connected,
		[],
	);

	return (
		<RealtimeContext.Provider value={{ connectionState, subscribe, invoke, isConnected }}>
			{children}
		</RealtimeContext.Provider>
	);
};

export default RealtimeProvider;
