import { useRef } from "react";
import { Route, Routes } from "react-router-dom";
import "./App.css";
import ModalProvider from "./components/contexts/modals/Modal-provider";
import Footer from "./components/layout/Footer";
import Nav from "./components/layout/Nav";
import Home from "./pages/home/Home";
import NotFound from "./pages/Not-found";

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
		<>
			<ModalProvider>
				<Nav onServicesClicked={scrollToServices} onHomeButtonClicked={scrollToHomeSection} />
				<Routes>
					<Route path="/" element={<Home servicesRef={servicesRef} homeRef={homeRef} />} />
					{/* <Route path="/about" element={<h1>About</h1>} /> */}
					{/* <Route path="/contact" element={<h1>Contact</h1>} /> */}
					{/* <Route path="/faq" element={<h1>FAQ</h1>} /> */}
					<Route path="*" element={<NotFound />} />
				</Routes>
			</ModalProvider>

			<Footer />
		</>
	);
}

export default App;
