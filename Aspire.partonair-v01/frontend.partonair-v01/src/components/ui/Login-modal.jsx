import { useContext, useState } from "react";
import { useNavigate } from "react-router-dom";
import AuthContext from "../../contexts/auth/Auth-context";
import { signinApi } from "../../services/api";

const LoginModal = ({ open, onClose, switchModal }) => {
	const { login } = useContext(AuthContext);
	const navigate = useNavigate();

	const [form, setForm] = useState({ email: "", password: "" });
	const [error, setError] = useState(null);
	const [loading, setLoading] = useState(false);

	const handleChange = (e) => {
		setForm({ ...form, [e.target.name]: e.target.value });
		setError(null);
	};

	const handleSubmit = async (e) => {
		e.preventDefault();
		setError(null);
		setLoading(true);

		try {
			const data = await signinApi.login({ email: form.email, password: form.password });
			login(data);
			onClose();

			if (!data.profileId) {
				navigate("/profile/setup");
			} else {
				navigate("/profile");
			}
		} catch (err) {
			setError(err.status === 400 ? "Identifiants incorrects." : "Une erreur est survenue.");
		} finally {
			setLoading(false);
		}
	};

	if (!open) return null;

	return (
		<div className="fixed z-50 top-1/2 left-1/2 -translate-x-1/2 -translate-y-1/2 p-6 rounded-lg shadow-lg w-full max-w-md glass-panel shadow-glow">
			<div className="flex items-center justify-between border-b border-default pb-4 md:pb-5">
				<h3 className="text-lg font-medium text-heading">Connexion rapide</h3>
				<button
					onClick={onClose}
					type="button"
					className="text-body bg-transparent hover:bg-neutral-tertiary hover:text-heading rounded-base text-sm w-9 h-9 ms-auto inline-flex justify-center items-center">
					<svg className="w-5 h-5" aria-hidden="true" xmlns="http://www.w3.org/2000/svg" width="24" height="24" fill="none" viewBox="0 0 24 24">
						<path stroke="currentColor" strokeLinecap="round" strokeLinejoin="round" strokeWidth="2" d="M6 18 17.94 6M18 18 6.06 6" />
					</svg>
					<span className="sr-only">Fermer</span>
				</button>
			</div>

			<form onSubmit={handleSubmit} className="pt-4 md:pt-6">
				<div className="mb-4 text-left">
					<label htmlFor="email" className="block mb-2.5 text-sm font-medium text-left">
						Ton mail
					</label>
					<input
						type="email"
						id="email"
						name="email"
						value={form.email}
						onChange={handleChange}
						className="bg-transparent border border-default-medium text-heading text-sm rounded-base focus:ring-brand focus:border-brand block w-full px-3 py-2.5 shadow-xs placeholder:text-body"
						placeholder="example@company.com"
						required
					/>
				</div>
				<div className="mb-6 text-left">
					<label htmlFor="password" className="block mb-2.5 text-sm font-medium text-left">
						Ton pass
					</label>
					<input
						type="password"
						id="password"
						name="password"
						value={form.password}
						onChange={handleChange}
						className="bg-transparent border border-default-medium text-heading text-sm rounded-base focus:ring-brand focus:border-brand block w-full px-3 py-2.5 shadow-xs placeholder:text-body"
						placeholder="•••••••••"
						required
					/>
				</div>

				{error && <p className="text-red-400 text-sm mb-4">{error}</p>}

				<button type="submit" disabled={loading} className="button-primary mb-2">
					{loading ? "Connexion..." : "Connexion"}
				</button>
				<div className="text-sm font-medium text-body">
					Pas de compte ?{" "}
					<a onClick={switchModal} href="#" className="text-fg-brand hover:underline hover:text-electric-300">
						Créer un compte
					</a>
				</div>
			</form>
		</div>
	);
};

export default LoginModal;
