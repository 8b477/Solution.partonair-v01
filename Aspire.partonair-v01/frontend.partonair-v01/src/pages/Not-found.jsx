import NotFound from "../assets/images/not-found.svg";

const notFound = () => {
	return (
		<div className="flex flex-col items-center justify-center h-screen">
			<img className="w-2/3" src={NotFound} alt="Image représentant une page non référencer par le site" />
			<h1 className="font-mono uppercase text-4xl bg-black">La page rechercher est introuvable !!!</h1>
		</div>
	);
};

export default notFound;
