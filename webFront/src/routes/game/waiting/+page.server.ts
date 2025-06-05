import { API_URL_PREFIX } from '$env/static/private';
import { redirect } from '@sveltejs/kit';

export async function load({ locals, fetch }) {
	const users = await fetch(`http://${API_URL_PREFIX}/gamestate`); // Replace with your actual data source

	if ((await users.json()) === 'Active') {
		return redirect(303, '/');
	}

	return {
		user: locals.user
	};
}
