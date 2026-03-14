import { useContext } from "react";
import HeroImage from "../../assets/images/hero-image.png";
import ModalContext from "../../components/contexts/modals/Modal-context";

const HeroSection = ({ homeRef }) => {
	const { registerModalHandler } = useContext(ModalContext);

	return (
		<section id="home" ref={homeRef}>
			<div className="relative z-10 max-w-7xl mx-auto px-6 lg:px-8 grid grid-cols-1 lg:grid-cols-2 gap-12 items-center py-20">
				<div className="space-y-8">
					<div className="inline-flex items-center gap-2 px-4 py-2 rounded-full glass-panel border-electric-500/30 mt-3">
						<span className="w-2 h-2 rounded-full bg-emerald-300 animate-pulse-glow"></span>
						<span className="text-xs font-semibold tracking-wider uppercase text-lime-50">100 % GRATUIT</span>
					</div>
					<h1 className="text-6xl md:text-7xl lg:text-8xl font-bold leading-tight tracking-tight">
						<div className="flex justify-center">
							<span className="block text-white">L'</span>
							<span className="block text-gradient">uni</span>
							<span>on</span>
						</div>
						<div className="flex justify-center">
							<span className="block text-gradient">F</span>
							<span className="block text-white">ait</span>
						</div>
						<div className="flex justify-center">
							<span className="block text-white">La f</span>
							<span className="block text-gradient">orce !</span>
						</div>
					</h1>
					<p className="text-xl text-gray-400 max-w-lg leading-relaxed">
						Le premier écosystème numérique qui met en avant et en relation des startups ambitieuses uniquement Walonne
						avec des talents créatifs et techniques d'élite.
					</p>
					<p className="text-xl text-gray-300"> Zéro frais, potentiel infini.</p>
					<div className="flex justify-center sm:flex-row gap-4 pt-4">
						<button onClick={registerModalHandler} className="button-secondary-big">
							Rejoins le réseau
						</button>
						<button className="button-primary-big">Explore Ecosystem</button>
					</div>
					<div className="pt-8 border-t border-white/5">
						<p className="text-sm text-gray-500 mb-4 font-mono uppercase tracking-widest">
							Entreprise et Talent, les meilleurs sont ici.
						</p>
						<div className="flex bg-black justify-center gap-6 text-gray-400 font-bold text-lg">
							<span className="hover:text-white transition-colors cursor-default">Consulte</span>
							<span className="text-gray-700">/</span>
							<span className="hover:text-white transition-colors cursor-default">Recherche</span>
							<span className="text-gray-700">/</span>
							<span className="hover:text-white transition-colors cursor-default">Crée</span>
							<span className="text-gray-700">/</span>
							<span className="hover:text-white transition-colors cursor-default">Contact</span>
						</div>
					</div>
				</div>
				<div className="relative h-[600px] hidden lg:block perspective-1000">
					<div className="absolute top-1/2 left-1/2 -translate-x-1/2 -translate-y-1/2 w-full h-full">
						<img
							alt="Modern networking visual"
							className="[mask-image:linear-gradient(to_bottom,black_50%,transparent_100%)] [-webkit-mask-image:linear-gradient(to_bottom,black_50%,transparent_100%)] w-full h-full object-cover rounded-3xl opacity-60"
							src={HeroImage}
						/>
						{/* Cards flottantes */}
						<div className="absolute top-20 right-10 glass-panel p-6 rounded-2xl w-64 animate-[float_6s_ease-in-out_infinite]">
							<div className="flex items-center gap-3 mb-3">
								<div className="w-10 h-10 rounded-full bg-yellow-400/20 flex items-center justify-center text-yellow-400">
									<span className="material-symbols-outlined">Bolt</span>
								</div>
								<div>
									<div className="text-sm font-bold text-white">New Startup</div>
									<div className="text-xs text-gray-400">Just joined</div>
								</div>
							</div>
							<div className="h-2 bg-white/10 rounded-full w-full overflow-hidden">
								<div className="h-full bg-yellow-400 w-3/4"></div>
							</div>
						</div>
						<div className="absolute bottom-40 left-0 glass-panel p-6 rounded-2xl w-72 animate-[float_7s_ease-in-out_infinite_reverse]">
							<div className="flex justify-between items-start mb-4">
								<div>
									<div className="text-2xl font-bold text-white">1.2k+</div>
									<div className="text-xs text-gray-400 uppercase tracking-wider">Active Talents</div>
								</div>
								<span className="text-slate-800 text-3xl material-symbols-outlined">diversity_1</span>
							</div>
							<div className="flex -space-x-2">
								<div className="w-8 h-8 rounded-full bg-gray-600 border-2 border-gray-900"></div>
								<div className="w-8 h-8 rounded-full bg-gray-500 border-2 border-gray-900"></div>
								<div className="w-8 h-8 rounded-full bg-gray-400 border-2 border-gray-900"></div>
								<div className="w-8 h-8 rounded-full bg-gray-800 border-2 border-gray-900 flex items-center justify-center text-[10px] text-white font-bold">
									+99
								</div>
							</div>
						</div>
					</div>
				</div>
			</div>
		</section>
	);
};

export default HeroSection;
