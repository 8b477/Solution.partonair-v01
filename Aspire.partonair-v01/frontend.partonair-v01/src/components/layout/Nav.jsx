import { useContext, useState } from "react";
import { NavLink, useNavigate } from "react-router-dom";
import Logo from "../../assets/images/logo-partonair.svg";
import AuthContext from "../../contexts/auth/Auth-context";
import ModalContext from "../contexts/modals/Modal-context";
import NotificationsContext from "../../contexts/notifications/Notifications-context";
import ProfileAvatar from "../ui/Profile-avatar";

const CONNECTED_LINKS = [
	{ to: "/profiles", label: "Découvre les profils" },
	{ to: "/network", label: "Mon réseau" },
	{ to: "/messages", label: "Mes messages" },
];

const connectedLinkClass = ({ isActive }) =>
	`block py-2 px-3 rounded-sm text-sm font-medium transition md:p-0 ${
		isActive
			? "text-white bg-electric-600 md:bg-transparent md:text-electric-400"
			: "text-white bg-electric-600/70 hover:text-electric-300 md:bg-transparent md:text-electric-500 md:dark:text-electric-400"
	}`;

const avatarLinkClass = ({ isActive }) =>
	`rounded-full ring-2 ring-offset-2 ring-offset-gray-900 transition focus-visible:outline-none focus-visible:ring-electric-300 ${
		isActive ? "ring-electric-400" : "ring-transparent hover:ring-electric-400/70"
	}`;

const Nav = ({ onServicesClicked, onHomeButtonClicked }) => {
	const { loginModalHandler, registerModalHandler } = useContext(ModalContext);
	const { auth, logout } = useContext(AuthContext);
	const { incomingCount, unreadTotal } = useContext(NotificationsContext);
	// Compteurs « non consultés » affichés à côté des liens.
	const badges = {
		"/network": { count: incomingCount, label: (n) => `${n} demande${n > 1 ? "s" : ""} en attente` },
		"/messages": { count: unreadTotal, label: (n) => `${n} message${n > 1 ? "s" : ""} non lu${n > 1 ? "s" : ""}` },
	};
	const navigate = useNavigate();
	const [mobileOpen, setMobileOpen] = useState(false);

	const closeMobileMenu = () => setMobileOpen(false);

	const handleLogout = () => {
		closeMobileMenu();
		logout();
		navigate("/");
	};

	const handleHomeClick = (event) => {
		closeMobileMenu();
		onHomeButtonClicked?.(event);
	};

	const handleServicesClick = (event) => {
		closeMobileMenu();
		onServicesClicked?.(event);
	};

	return (
		<header>
			<nav className="dark:bg-gray-900 fixed w-full z-20 top-0 start-0 border-b border-gray-200 dark:border-gray-600">
				<div className="max-w-screen-xl flex flex-wrap items-center justify-between mx-auto">
					<a onClick={handleHomeClick} href="#home" className="flex items-center space-x-3 rtl:space-x-reverse">
						<img src={Logo} alt="Partonair Logo" />
					</a>

					<div className="flex md:order-2 items-center space-x-3 md:space-x-0 rtl:space-x-reverse">
						{auth ? (
							<div className="flex items-center gap-4">
								<NavLink
									to="/profile"
									onClick={closeMobileMenu}
									className={avatarLinkClass}
									aria-label="Mon profil"
									title={auth.userName}>
									<ProfileAvatar name={auth.userName} seed={auth.avatarSeed} className="h-10 w-10 text-sm" />
								</NavLink>
								<span className="h-6 w-px bg-white/15" aria-hidden="true" />
								<button onClick={handleLogout} className="button-primary me-0 text-sm" type="button">
									Déconnexion
								</button>
							</div>
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
							onClick={() => setMobileOpen((open) => !open)}
							type="button"
							className="inline-flex items-center p-2 w-10 h-10 justify-center text-sm text-gray-500 rounded-lg md:hidden hover:bg-gray-100 focus:outline-none focus:ring-2 focus:ring-gray-200 dark:text-gray-400 dark:hover:bg-gray-700 dark:focus:ring-gray-600"
							aria-controls="navbar-sticky"
							aria-expanded={mobileOpen}>
							<span className="sr-only">{mobileOpen ? "Fermer le menu" : "Ouvrir le menu"}</span>
							<svg className="w-5 h-5" aria-hidden="true" xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 17 14">
								<path stroke="currentColor" strokeLinecap="round" strokeLinejoin="round" strokeWidth="2" d="M1 1h15M1 7h15M1 13h15" />
							</svg>
						</button>
					</div>

					<div
						className={`w-full md:flex md:w-auto md:order-1 ${mobileOpen ? "block" : "hidden"}`}
						id="navbar-sticky">
						{auth ? (
							<ul className="flex flex-col p-4 md:p-0 mt-4 font-medium border border-gray-100 rounded-lg bg-gray-50 md:space-x-8 rtl:space-x-reverse md:flex-row md:mt-0 md:border-0 md:bg-white dark:bg-gray-800 md:dark:bg-gray-900 dark:border-gray-700">
								{CONNECTED_LINKS.map((link) => {
									const badge = badges[link.to];
									return (
										<li key={link.to}>
											<NavLink to={link.to} onClick={closeMobileMenu} className={connectedLinkClass}>
												<span className="relative inline-flex items-center">
													{link.label}
													{badge?.count > 0 && (
														<span
															className="ml-1.5 inline-flex h-5 min-w-5 items-center justify-center rounded-full bg-red-500 px-1.5 text-[0.65rem] font-bold leading-none text-white"
															aria-label={badge.label(badge.count)}>
															{badge.count > 9 ? "9+" : badge.count}
														</span>
													)}
												</span>
											</NavLink>
										</li>
									);
								})}
							</ul>
						) : (
							<ul className="flex flex-col p-4 md:p-0 mt-4 font-medium border border-gray-100 rounded-lg bg-gray-50 md:space-x-8 rtl:space-x-reverse md:flex-row md:mt-0 md:border-0 md:bg-white dark:bg-gray-800 md:dark:bg-gray-900 dark:border-gray-700">
								<li>
									<a
										href="#"
										className="block py-2 px-3 text-white bg-electric-600 hover:text-electric-300 rounded-sm md:bg-transparent md:text-electric-500 md:p-0 md:dark:text-electric-400"
										aria-current="page"
										onClick={handleHomeClick}>
										Accueil
									</a>
								</li>
								<li>
									<a
										href="#services"
										className="block py-2 px-3 text-white bg-electric-600 hover:text-electric-300 rounded-sm md:bg-transparent md:text-electric-500 md:p-0 md:dark:text-electric-400"
										onClick={handleServicesClick}>
										Services
									</a>
								</li>
							</ul>
						)}
					</div>
				</div>
			</nav>
		</header>
	);
};

export default Nav;
