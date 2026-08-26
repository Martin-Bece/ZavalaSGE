export const materias = [
	{
		id: 1,
		nombre: 'Matemática I',
		codigo: 'MAT-01',
		curso: 'Módulo 1'
	},
	{
		id: 2,
		nombre: 'Programación Web',
		codigo: 'PRG-02',
		curso: 'Módulo 2'
	},
	{
		id: 3,
		nombre: 'Sistemas Operativos',
		codigo: null, // Caso incompleto: materia sin código asignado
		curso: 'Módulo 2'
	}
];

export function obtener_materias() {
	return new Promise((resolve) => {
		setTimeout(() => {
			resolve(materias);
		}, 600);
	});
}