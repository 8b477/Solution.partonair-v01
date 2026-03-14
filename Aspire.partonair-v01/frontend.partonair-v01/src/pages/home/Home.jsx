import HeroSection from "./Hero-section";
import ServiceSection from "./Services-section";
const Home = ({ servicesRef, homeRef }) => {
	return (
		<>
			<div className="w-full bg-gradient-to-br from-electric-600 via-[#050816] to-indigo-900">
				<main>
					<HeroSection homeRef={homeRef} />
				</main>
				<hr />
				<ServiceSection servicesRef={servicesRef} />
			</div>
		</>
	);
};

export default Home;
