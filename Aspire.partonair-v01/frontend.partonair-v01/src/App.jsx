import { useRef } from "react";
import { Route, Routes } from "react-router-dom";
import "./App.css";
import AuthProvider from "./contexts/auth/Auth-provider";
import ModalProvider from "./components/contexts/modals/Modal-provider";
import Footer from "./components/layout/Footer";
import Nav from "./components/layout/Nav";
import Home from "./pages/home/Home";
import NotFound from "./pages/Not-found";
import Profile from "./pages/profile/Profile";
import ProfileSetup from "./pages/profile/Profile-setup";

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
			<ModalProvider>
				<Nav onServicesClicked={scrollToServices} onHomeButtonClicked={scrollToHomeSection} />
				<Routes>
					<Route path="/" element={<Home servicesRef={servicesRef} homeRef={homeRef} />} />
					<Route path="/profile/setup" element={<ProfileSetup />} />
					<Route path="/profile" element={<Profile />} />
					<Route path="*" element={<NotFound />} />
				</Routes>
			</ModalProvider>

			<Footer />
		</AuthProvider>
	);
}

export default App;
