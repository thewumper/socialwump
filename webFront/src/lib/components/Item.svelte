<script>
	const { itemID } = $props();

	let firstLetter = $state('');
	$effect(() => {
		firstLetter = itemID[0];
	});
	let name = '';
	$effect(() => {
		name = String(itemID).split('_')[1];
	});

	let bgColor = $state('#000000');

	if (firstLetter === 'a') {
		bgColor = '#ff0000';
	}
</script>

<div class="flex flex-col">
	<div
		class="h-20 w-20 rounded-full border-2 border-white p-2"
		style="background-color: {bgColor}"
	></div>
	<span>{name}</span>
</div>
