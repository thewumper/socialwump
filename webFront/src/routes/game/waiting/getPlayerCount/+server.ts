import { API_URL_PREFIX } from '$env/static/private';

export async function GET(event) {
	// Logic to fetch users (e.g., from a database)
	const playercount = await event.fetch(`http://${API_URL_PREFIX}/PlayersInGame`); // Replace with your actual data source
	const json = await playercount.json();
	return new Response(JSON.stringify(json), {
		headers: {
			'Content-Type': 'application/json'
		}
	});
}
