import { redirect } from '@sveltejs/kit';

export async function load({ locals, request, fetch }) {
	// Logic to fetch users (e.g., from a database)

	console.log('HI');

	if (locals.user) {
		return redirect(303, '/game');
	} else {
		return redirect(303, '/account/login');
	}
}
