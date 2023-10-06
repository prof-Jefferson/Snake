using System.Collections.Generic;
using UnityEngine;

public class Snake
{
	public List<Vector2Int> segments = new List<Vector2Int>();
	public Vector2Int direction;
	
	public int score = 0;
	
	public Snake(Vector2Int initialPosition, Vector2Int initialDirection)
	{
		segments.Add(initialPosition); // Inicialmente, a cobrinha tem um segmento (a cabe�a).
		direction = initialDirection;
	}

	public void Move()
	{
		// Move a cabe�a na dire��o atual.
		Vector2Int newHeadPosition = segments[0] + direction;

		// Adiciona o novo segmento da cabe�a na lista.
		segments.Insert(0, newHeadPosition);

		// Remove o �ltimo segmento (a "cauda") - isso simula o movimento.
		// Se a cobrinha crescer (comer uma ma��), n�s n�o queremos remover o �ltimo segmento neste movimento.
		// Isso faz parecer que a cobrinha cresceu por um segmento.
		if (willGrow)
		{
			willGrow = false; // Reseta a flag.
		}
		else
		{
			segments.RemoveAt(segments.Count - 1);
		}
	}

	private bool willGrow = false;

	public void Grow()
	{
		willGrow = true; // A pr�xima vez que a cobrinha se mover, ela n�o perder� seu �ltimo segmento.
		score++;		
	}
}
