<script>
	import Item from './Item.svelte';
</script>

<div class="flex flex-col gap-1">
	{#each { length: 5 } as _, i}
		<!-- <Item itemID="1" /> -->
	{/each}
</div>
