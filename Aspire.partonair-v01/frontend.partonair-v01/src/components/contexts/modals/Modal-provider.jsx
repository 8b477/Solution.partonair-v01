import { useState } from "react";
import LoginModal from "../../ui/Login-modal";
import RegisterModal from "../../ui/Register-modal";
import ModalContext from "./Modal-context";

const ModalProvider = ({ children }) => {
	const [isLoginModalOpen, setIsLoginModalOpen] = useState(false);
	const [isRegisterModalOpen, setIsRegisterModalOpen] = useState(false);

	function loginModalHandler() {
		setIsLoginModalOpen(!isLoginModalOpen);
		setIsRegisterModalOpen(false);
	}

	function registerModalHandler() {
		setIsRegisterModalOpen(!isRegisterModalOpen);
		setIsLoginModalOpen(false);
	}

	const value = {
		isLoginModalOpen,
		loginModalHandler,
		isRegisterModalOpen,
		registerModalHandler,
	};

	return (
		<ModalContext.Provider value={value}>
			{children}
			<LoginModal open={isLoginModalOpen} onClose={loginModalHandler} switchModal={registerModalHandler} />
			<RegisterModal open={isRegisterModalOpen} onClose={registerModalHandler} switchModal={loginModalHandler} />
		</ModalContext.Provider>
	);
};

export default ModalProvider;
