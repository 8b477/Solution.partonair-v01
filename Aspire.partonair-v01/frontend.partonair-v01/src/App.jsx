import { useRef } from "react";
import { Route, Routes } from "react-router-dom";
import "./App.css";
import AuthProvider from "./contexts/auth/Auth-provider";
import RealtimeProvider from "./contexts/realtime/Realtime-provider";
import NotificationsProvider from "./contexts/notifications/Notifications-provider";
import ModalProvider from "./components/contexts/modals/Modal-provider";
import Footer from "./components/layout/Footer";
import Nav from "./components/layout/Nav";
import Home from "./pages/home/Home";
import NotFound from "./pages/Not-found";
import Profile from "./pages/profile/Profile";
import ProfileSetup from "./pages/profile/Profile-setup";
import Profiles from "./pages/profiles/Profiles";
import ProfileDetails from "./pages/profiles/Profile-details";
import Network from "./pages/network/Network";
import Messages from "./pages/messages/Messages";

function App() {
	const servicesRef = useRef(null);
	const homeRef = useRef(null);

	const scrollToServices = (e) => {
		e.preventDefault();
		servicesRef.current?.scrollIntoView({ behavior: "smooth" });
	};

	const scrollToHomeSection = (e) => {
		e.preventDefault();
		homeRef.current?.scrollIntoView({ behavior: "smooth" });
	};

	return (
		<AuthProvider>
			<RealtimeProvider>
				<NotificationsProvider>
					<ModalProvider>
						<Nav onServicesClicked={scrollToServices} onHomeButtonClicked={scrollToHomeSection} />
						<Routes>
							<Route path="/" element={<Home servicesRef={servicesRef} homeRef={homeRef} />} />
							<Route path="/profile/setup" element={<ProfileSetup />} />
							<Route path="/profile" element={<Profile />} />
							<Route path="/profiles" element={<Profiles />} />
							<Route path="/profiles/:profileId" element={<ProfileDetails />} />
							<Route path="/network" element={<Network />} />
							<Route path="/messages" element={<Messages />} />
							<Route path="/messages/:friendId" element={<Messages />} />
							<Route path="*" element={<NotFound />} />
						</Routes>
					</ModalProvider>
				</NotificationsProvider>

				<Footer />
			</RealtimeProvider>
		</AuthProvider>
	);
}

export default App;
