import { redirect } from '@sveltejs/kit';

function splitByClassType(items) {
	return items.reduce((result, item) => {
		const classKey = item.classType;
		if (!result[classKey]) {
			result[classKey] = [];
		}
		result[classKey].push(item);
		return result;
	}, {});
}
export async function load({ locals, fetch }) {
	const users = await fetch('http://wumpapi:8080/gamestate'); // Replace with your actual data source

	if ((await users.json()) === 'Waiting') {
		return redirect(303, '/game/waiting');
	}

	const shopinforesp = await fetch('/game/shop/getitemlist');
	const json = await shopinforesp.json();

	const splitItems = splitByClassType(json);

	return {
		playerJoined: false,
		user: locals.user,
		shopItems: splitItems
	};
}
