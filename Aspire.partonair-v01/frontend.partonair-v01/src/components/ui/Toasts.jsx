import { Link } from "react-router-dom";
import ProfileAvatar from "./Profile-avatar";

// Pile de notifications éphémères en bas à droite de l'écran.
const Toasts = ({ toasts, onDismiss }) => (
	<div className="pointer-events-none fixed bottom-4 right-4 z-50 flex w-[calc(100%-2rem)] max-w-sm flex-col gap-3" aria-live="polite">
		{toasts.map((toast) => (
			<div
				key={toast.id}
				role="status"
				className="glass-panel pointer-events-auto flex items-start gap-3 rounded-2xl border-electric-300/30 bg-[#0B1224]/90 p-4 text-left shadow-glow">
				{toast.avatar && <ProfileAvatar name={toast.avatar.name} seed={toast.avatar.seed} className="h-10 w-10 text-sm" />}
				<div className="min-w-0 flex-1">
					<p className="text-sm font-semibold text-heading">{toast.title}</p>
					{toast.message && <p className="mt-0.5 text-sm text-body">{toast.message}</p>}
					{toast.action && (
						<Link
							to={toast.action.to}
							onClick={() => onDismiss(toast.id)}
							className="mt-2 inline-flex text-sm font-semibold text-electric-300 hover:text-electric-400">
							{toast.action.label} <span aria-hidden="true">→</span>
						</Link>
					)}
				</div>
				<button
					type="button"
					onClick={() => onDismiss(toast.id)}
					className="rounded-md p-1 text-body transition hover:bg-white/10 hover:text-heading"
					aria-label="Fermer la notification">
					✕
				</button>
			</div>
		))}
	</div>
);

export default Toasts;
