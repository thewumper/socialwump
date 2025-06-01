export async function GET(event) {
	// Logic to fetch users (e.g., from a database)
	const items = await event.fetch('http://127.0.0.1:42069/iteminfo'); // Replace with your actual data source
	const json = await items.json();
	return new Response(JSON.stringify(json), {
		headers: {
			'Content-Type': 'application/json'
		}
	});
}
