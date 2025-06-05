export async function GET(event) {
	// Logic to fetch users (e.g., from a database)
	const playercount = await event.fetch('http://wumpapi:8080/PlayersInGame'); // Replace with your actual data source
	const json = await playercount.json();
	return new Response(JSON.stringify(json), {
		headers: {
			'Content-Type': 'application/json'
		}
	});
}
