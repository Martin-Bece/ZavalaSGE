export const comisiones = [
	{
		id: 1,
		nombre: 'Comisión A',
		curso: 'Módulo 1',
		preceptor: 'Laura Gómez'
	},
	{
		id: 2,
		nombre: 'Comisión B',
		curso: 'Módulo 2',
		preceptor: 'Carlos Rodríguez'
	},
	{
		id: 3,
		nombre: 'Comisión C',
		curso: 'Módulo 3',
		preceptor: null // Caso incompleto: comisión sin preceptor asignado
	}
];

export function obtener_comisiones() {
	return new Promise((resolve) => {
		setTimeout(() => {
			resolve(comisiones);
		}, 600);
	});
}