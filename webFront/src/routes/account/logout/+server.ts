import { redirect } from '@sveltejs/kit';

export async function GET(event) {
	// Logic to fetch users (e.g., from a database)
	const users = await event.fetch('http://127.0.0.1:8080/maxWantsADummyBecauseHeIsADummy'); // Replace with your actual data source

	return redirect(302, '/');
}
