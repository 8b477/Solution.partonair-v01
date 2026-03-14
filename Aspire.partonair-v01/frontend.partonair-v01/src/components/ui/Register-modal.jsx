const RegisterModal = ({ open, onClose, switchModal }) => {
	if (!open) return null;
	return (
		<>
			<div className="fixed z-50 top-1/2 left-1/2 -translate-x-1/2 -translate-y-1/2 p-6 rounded-lg shadow-lg w-full max-w-md glass-panel shadow-glow">
				<div className="flex items-center justify-between border-b border-default pb-4 md:pb-5">
					<h3 className="text-lg font-medium text-heading">Inscription rapide</h3>
					<button
						onClick={onClose}
						type="button"
						className="text-body bg-transparent hover:bg-neutral-tertiary hover:text-heading rounded-base text-sm w-9 h-9 ms-auto inline-flex justify-center items-center">
						<svg
							className="w-5 h-5"
							aria-hidden="true"
							xmlns="http://www.w3.org/2000/svg"
							width="24"
							height="24"
							fill="none"
							viewBox="0 0 24 24">
							<path
								stroke="currentColor"
								strokeLinecap="round"
								strokeLinejoin="round"
								strokeWidth="2"
								d="M6 18 17.94 6M18 18 6.06 6"
							/>
						</svg>
						<span className="sr-only">Fermer</span>
					</button>
				</div>

				<form action="#" className="pt-4 md:pt-6">
					<div className="mb-4 text-left">
						<label for="email" className="block mb-2.5 text-sm font-medium text-left">
							Ton mail
						</label>
						<input
							type="email"
							id="email"
							className="bg-transparent border border-default-medium text-heading text-sm rounded-base focus:ring-brand focus:border-brand block w-full px-3 py-2.5 shadow-xs placeholder:text-body"
							placeholder="example@company.com"
							required
						/>
					</div>
					<div>
						<label for="password" className="block mb-2.5 text-sm font-medium text-left">
							Ton pass
						</label>
						<input
							type="password"
							id="password"
							className="bg-transparent border border-default-medium text-heading text-sm rounded-base focus:ring-brand focus:border-brand block w-full px-3 py-2.5 shadow-xs placeholder:text-body"
							placeholder="•••••••••"
							required
						/>
					</div>
					<div className="flex items-start my-6">
						<div className="flex items-center">
							<input id="checkbox-remember" type="checkbox" value="" className="bg-transparent" />
							<label for="checkbox-remember" className="ms-2 text-sm font-medium text-heading">
								Se souvenir de moi
							</label>
						</div>
					</div>
					<button type="submit" className="button-primary mb-2">
						Inscription
					</button>
					<div className="text-sm font-medium text-body">
						Déjà inscrit ?{" "}
						<a onClick={switchModal} href="#" className="text-fg-brand hover:underline hover:text-electric-300">
							Connecte toi ici !
						</a>
					</div>
				</form>
			</div>
		</>
	);
};

export default RegisterModal;
