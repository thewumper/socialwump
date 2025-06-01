export async function POST({ locals, request, fetch }) {
	// Logic to fetch users (e.g., from a database)

	const formdata = await request.formData();

	const data = formdata.get('targetUser');

	const users = await fetch('http://127.0.0.1:42069/createRelationship', {
		method: 'POST',
		body: JSON.stringify({
			SessionToken: locals.sessionID,
			RelationshipName: 'Friends',
			TargetUser: data,
			Data: ''
		}),
		headers: {
			'Content-Type': 'application/json'
		}
	}); // Replace with your actual data source

	// const json = await users.json();
	// console.log(json);
	return new Response(JSON.stringify({}), {
		headers: {
			'Content-Type': 'application/json'
		}
	});
}
