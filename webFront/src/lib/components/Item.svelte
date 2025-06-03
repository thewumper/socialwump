<script>
	const { itemInfo } = $props();
</script>

<div class="flex flex-col">
	<div
		class="h-16 w-16 rounded-full border-2 border-black p-2"
		style="background-color: #bb33ff"
	></div>
	<span>{itemInfo.name}</span>
	<span>{itemInfo.price}</span>
	<span>{itemInfo.buildTime}s</span>
	<span>{itemInfo.description}</span>
	<!--  TODO! Format this nicelys -->
</div>
