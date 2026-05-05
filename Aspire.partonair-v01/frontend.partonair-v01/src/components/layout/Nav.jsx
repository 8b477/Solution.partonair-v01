import { useContext } from "react";
import { useNavigate } from "react-router-dom";
import Logo from "../../assets/images/logo-partonair.svg";
import AuthContext from "../../contexts/auth/Auth-context";
import ModalContext from "../contexts/modals/Modal-context";

const Nav = ({ onServicesClicked, onHomeButtonClicked }) => {
	const { loginModalHandler, registerModalHandler } = useContext(ModalContext);
	const { auth, logout } = useContext(AuthContext);
	const navigate = useNavigate();

	const handleLogout = () => {
		logout();
		navigate("/");
	};

	return (
		<header>
			<nav className="dark:bg-gray-900 fixed w-full z-20 top-0 start-0 border-b border-gray-200 dark:border-gray-600">
				<div className="max-w-screen-xl flex flex-wrap items-center justify-between mx-auto">
					<a onClick={onHomeButtonClicked} href="#home" className="flex items-center space-x-3 rtl:space-x-reverse">
						<img src={Logo} alt="Partonair Logo" />
					</a>

					<div className="flex md:order-2 items-center space-x-3 md:space-x-0 rtl:space-x-reverse">
						{auth ? (
							<>
								<button
									onClick={() => navigate("/profile")}
									className="button-secondary text-sm">
									{auth.userName}
								</button>
								<button onClick={handleLogout} className="button-primary text-sm" type="button">
									Déconnexion
								</button>
							</>
						) : (
							<>
								<button onClick={loginModalHandler} className="button-primary" type="button">
									Connexion
								</button>
								<button onClick={registerModalHandler} className="button-secondary" type="button">
									Inscription
								</button>
							</>
						)}

						{/* Mobile button */}
						<button
							data-collapse-toggle="navbar-sticky"
							type="button"
							className="inline-flex items-center p-2 w-10 h-10 justify-center text-sm text-gray-500 rounded-lg md:hidden hover:bg-gray-100 focus:outline-none focus:ring-2 focus:ring-gray-200 dark:text-gray-400 dark:hover:bg-gray-700 dark:focus:ring-gray-600"
							aria-controls="navbar-sticky"
							aria-expanded="false">
							<span className="sr-only">Open main menu</span>
							<svg className="w-5 h-5" aria-hidden="true" xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 17 14">
								<path stroke="currentColor" strokeLinecap="round" strokeLinejoin="round" strokeWidth="2" d="M1 1h15M1 7h15M1 13h15" />
							</svg>
						</button>
					</div>

					<div className="items-center justify-between hidden w-full md:flex md:w-auto md:order-1" id="navbar-sticky">
						<ul className="flex flex-col p-4 md:p-0 mt-4 font-medium border border-gray-100 rounded-lg bg-gray-50 md:space-x-8 rtl:space-x-reverse md:flex-row md:mt-0 md:border-0 md:bg-white dark:bg-gray-800 md:dark:bg-gray-900 dark:border-gray-700">
							<li>
								<a
									href="#"
									className="block py-2 px-3 text-white bg-electric-600 hover:text-electric-300 rounded-sm md:bg-transparent md:text-electric-500 md:p-0 md:dark:text-electric-400"
									aria-current="page"
									onClick={onHomeButtonClicked}>
									Accueil
								</a>
							</li>
							<li>
								<a
									href="#services"
									className="block py-2 px-3 text-white bg-electric-600 hover:text-electric-300 rounded-sm md:bg-transparent md:text-electric-500 md:p-0 md:dark:text-electric-400"
									onClick={onServicesClicked}>
									Services
								</a>
							</li>
							<li>
								<a href="#" className="block py-2 px-3 text-white bg-electric-600 hover:text-electric-300 rounded-sm md:bg-transparent md:text-electric-500 md:p-0 md:dark:text-electric-400">
									Contacts
								</a>
							</li>
							<li>
								<a href="#" className="block py-2 px-3 text-white bg-electric-600 hover:text-electric-300 rounded-sm md:bg-transparent md:text-electric-500 md:p-0 md:dark:text-electric-400">
									FAQ
								</a>
							</li>
						</ul>
					</div>
				</div>
			</nav>
		</header>
	);
};

export default Nav;
