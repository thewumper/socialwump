export const handle = async ({ event, resolve }) => {
	const requestedPath = event.url.pathname;
	const cookies = event.cookies;

	// Auth check will go here
	const currentToken = cookies.get('auth-token');

	const authStatus = await event.fetch('http://127.0.0.1:8080/maxWantsADummyBecauseHeIsADummy'); // Replace with your actual data source

	console.log(requestedPath);

	const response = await resolve(event);

	return response;
};
