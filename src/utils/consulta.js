export function construir_query(parametros = {}){
	const query = new URLSearchParams();
	Object.keys(parametros).forEach(clave =>{
		const valor = parametros[clave];
		// null, undefined y '' significan "sin filtro" y no viajan. El false SI viaja: es el
		// filtro "no disponibles", que es distinto de no filtrar.
		if(valor === null || valor === undefined || valor === '') return;
		query.set(clave, valor);
	});
	const texto = query.toString();
	return texto ? `?${texto}` : '';
}