export async function GET(event) {
	// Logic to fetch users (e.g., from a database)
	const gamestate = await event.fetch('http://127.0.0.1:42069/gamestate');
	const json = await gamestate.json();
	return new Response(JSON.stringify(json), {
		headers: {
			'Content-Type': 'application/json'
		}
	});
}
