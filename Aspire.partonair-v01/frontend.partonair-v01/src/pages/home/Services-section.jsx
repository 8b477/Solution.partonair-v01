const ServicesSection = ({ servicesRef }) => {
	return (
		<>
			<section id="services" ref={servicesRef} className="py-32 relative bg-black">
				<div className="max-w-7xl mx-auto px-6 lg:px-8">
					<div className="flex flex-col md:flex-row justify-between items-end mb-16 gap-8">
						<div>
							<h2 className="text-sm font-bold text-electric-400 tracking-widest uppercase mb-2">Les services</h2>
							<h3 className="text-4xl md:text-5xl font-bold text-white">
								Construire de <span className="text-gradient">nouvelle </span>
								<span className="text-gradient">colab</span>
								oration
							</h3>
						</div>
						<p className="text-gray-400 max-w-md text-lg text-right md:text-left">
							Une plateforme pour observer les mouvements du marché, trouver des talents, et collaborer sur des projets
							innovants.
						</p>
					</div>
					<div className="grid grid-cols-1 md:grid-cols-3 gap-6">
						<div className="group relative p-8 rounded-3xl bg-navy-800 border border-electric-300/40 hover:border-electric-500/70 hover:scale-105 transition-all duration-500 overflow-hidden">
							<div className="absolute inset-0 bg-gradient-to-br from-electric-500/10 to-transparent opacity-0 group-hover:opacity-100 transition-opacity duration-500"></div>
							<div className="relative z-10">
								<div className="w-14 h-14 rounded-2xl bg-navy-900 border border-white/10 flex items-center justify-center text-white mb-6 group-hover:scale-110 transition-transform duration-500 shadow-lg">
									<span className="material-symbols-outlined text-3xl">handshake</span>
								</div>
								<h4 className="relative text-xl font-bold text-white mb-3">Création d'annonce</h4>
								<p className="text-gray-400 leading-relaxed">
									Ajouter, modifier ou paramétrer vos annonces de projets et d'opportunités en quelques clics, avec des
									templates optimisés pour le marché belge.
								</p>
							</div>
						</div>
						<div className="group relative p-8 rounded-3xl bg-navy-800 border border-electric-300/40 hover:border-electric-500/70 hover:scale-105 transition-all duration-500 overflow-hidden">
							<div className="absolute inset-0 bg-gradient-to-br from-electric-500/10 to-transparent opacity-0 group-hover:opacity-100 transition-opacity duration-500"></div>
							<div className="relative z-10">
								<div className="w-14 h-14 rounded-2xl bg-navy-900 border border-white/10 flex items-center justify-center text-white mb-6 group-hover:scale-110 transition-transform duration-500 shadow-lg">
									<span className="material-symbols-outlined text-3xl">hub</span>
								</div>
								<h4 className="text-xl font-bold text-white mb-3">Smart Matching</h4>
								<p className="text-gray-400 leading-relaxed">
									Connexion algorithmique entre entreprises et talents basée sur les compétences, les intérêts et les
									objectifs de carrière.
								</p>
							</div>
						</div>
						<div className="group relative p-8 rounded-3xl bg-navy-800 border border-electric-300/40 hover:border-electric-500/70 hover:scale-105 transition-all duration-500 overflow-hidden">
							<div className="absolute inset-0 bg-gradient-to-br from-electric-500/10 to-transparent opacity-0 group-hover:opacity-100 transition-opacity duration-500"></div>
							<div className="relative z-10">
								<div className="w-14 h-14 rounded-2xl bg-navy-900 border border-white/10 flex items-center justify-center text-white mb-6 group-hover:scale-110 transition-transform duration-500 shadow-lg">
									<span className="material-symbols-outlined text-3xl">rocket_launch</span>
								</div>
								<h4 className="text-xl font-bold text-white mb-3">Growth Velocity</h4>
								<p className="text-gray-400 leading-relaxed">
									Notre réseau de mentors et d'experts est là pour booster votre croissance et vous aider à atteindre
									vos objectifs plus rapidement.
								</p>
							</div>
						</div>
					</div>
				</div>
				<div className="max-w-7xl mx-auto mt-3 px-6 lg:px-8">
					<div className="grid grid-cols-1 md:grid-cols-3 gap-6">
						<div className="group relative p-8 rounded-3xl bg-navy-800 border border-electric-300/40 hover:border-electric-500/70 hover:scale-105 transition-all duration-500 overflow-hidden">
							<div className="absolute inset-0 bg-gradient-to-br from-electric-500/10 to-transparent opacity-0 group-hover:opacity-100 transition-opacity duration-500"></div>
							<div className="relative z-10">
								<div className="w-14 h-14 rounded-2xl bg-navy-900 border border-white/10 flex items-center justify-center text-white mb-6 group-hover:scale-110 transition-transform duration-500 shadow-lg">
									<span className="material-symbols-outlined text-3xl">Groups</span>
								</div>
								<h4 className="relative text-xl font-bold text-white mb-3">Solidarité</h4>
								<p className="text-gray-400 leading-relaxed">
									Mettre en avant vos talent d'entreprenariat ou d'employé et trouver des opportunités de collaboration,
									de mentorat ou de soutien financier au sein de la communauté.
								</p>
							</div>
						</div>
						<div className="group relative p-8 rounded-3xl bg-navy-800 border border-electric-300/40 hover:border-electric-500/70 hover:scale-105 transition-all duration-500 overflow-hidden">
							<div className="absolute inset-0 bg-gradient-to-br from-electric-500/10 to-transparent opacity-0 group-hover:opacity-100 transition-opacity duration-500"></div>
							<div className="relative z-10">
								<div className="w-14 h-14 rounded-2xl bg-navy-900 border border-white/10 flex items-center justify-center text-white mb-6 group-hover:scale-110 transition-transform duration-500 shadow-lg">
									<span className="material-symbols-outlined text-3xl">robot_2</span>
								</div>
								<h4 className="text-xl font-bold text-white mb-3">Smart Matching</h4>
								<p className="text-gray-400 leading-relaxed">
									Connexion algorithmique entre entreprises et talents basée sur les compétences, les intérêts et les
									objectifs de carrière.
								</p>
							</div>
						</div>
						<div className="group relative p-8 rounded-3xl bg-navy-800 border border-electric-300/40 hover:border-electric-500/70 hover:scale-105 transition-all duration-500 overflow-hidden">
							<div className="absolute inset-0 bg-gradient-to-br from-electric-500/10 to-transparent opacity-0 group-hover:opacity-100 transition-opacity duration-500"></div>
							<div className="relative z-10">
								<div className="w-14 h-14 rounded-2xl bg-navy-900 border border-white/10 flex items-center justify-center text-white mb-6 group-hover:scale-110 transition-transform duration-500 shadow-lg">
									<span className="material-symbols-outlined text-3xl">video_chat</span>
								</div>
								<h4 className="text-xl font-bold text-white mb-3">Contact direct</h4>
								<p className="text-gray-400 leading-relaxed">
									Contactez directement les entreprises ou les talents qui vous intéressent via notre système de
									messagerie intégré, sans intermédiaires ni frais cachés.
								</p>
							</div>
						</div>
					</div>
				</div>
			</section>
			<hr />
		</>
	);
};

export default ServicesSection;
