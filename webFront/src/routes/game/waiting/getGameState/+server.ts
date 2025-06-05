export async function GET(event) {
	// Logic to fetch users (e.g., from a database)
	const gamestate = await event.fetch('http://wumpapi:8080/gamestate');
	const json = await gamestate.json();
	return new Response(JSON.stringify(json), {
		headers: {
			'Content-Type': 'application/json'
		}
	});
}
