using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using TMPro;

public class SnakeGameManager : MonoBehaviour
{
	public GameObject gameOverPanel;
	public TextMeshProUGUI scoreTextMeshPro;
	
	public GameObject blockPrefab;
	public Color snakeColor = new Color(1f, 1f, 1f, 0.8f); // 80% de opacidade
	public Color appleColor = new Color(1f, 0f, 0f, 0.8f); // Vermelho com 80% de opacidade

    public AudioSource backgroundMusic;
    public AudioSource appleSound;

    private Vector2[,] positions;
	private int[,] gameMatrix = new int[10, 20];

	private float startX = -1.596f;
	private float startY = 0.211f;
	private float offset = 0.211f;

	private Snake snake;
	private Vector2Int applePosition;
	private Dictionary<Vector2Int, GameObject> boardBlocks = new Dictionary<Vector2Int, GameObject>();

	void Start()
	{
		InitializeMatrix();
		InitializeSnake();
		PlaceApple();
		StartCoroutine(SnakeMovementRoutine());
        backgroundMusic.Play();
    }

	void InitializeMatrix()
	{
		positions = new Vector2[10, 20];

		for (int x = 0; x < 10; x++)
		{
			for (int y = 0; y < 20; y++)
			{
				positions[x, y] = new Vector2(startX + x * offset, startY + y * offset);

				GameObject block = Instantiate(blockPrefab, positions[x, y], Quaternion.identity);
				block.SetActive(false); // Desativado por padr�o.

				boardBlocks[new Vector2Int(x, y)] = block;
			}
		}
	}

	void InitializeSnake()
	{
		snake = new Snake(new Vector2Int(5, 10), new Vector2Int(1, 0));
	}

	void Update()
	{
		HandleInput();

		if (snake.segments[0] == applePosition)
		{
			snake.Grow(); // Faz a cobrinha crescer.
			PlaceApple(); // Reposiciona a ma��.
						  // E qualquer outra l�gica que voc� queira implementar quando a cobrinha come uma ma��.
		}
	}

	void HandleInput()
	{
		if (Input.GetKeyDown(KeyCode.UpArrow) && snake.direction.y == 0)
			snake.direction = new Vector2Int(0, 1);
		if (Input.GetKeyDown(KeyCode.DownArrow) && snake.direction.y == 0)
			snake.direction = new Vector2Int(0, -1);
		if (Input.GetKeyDown(KeyCode.LeftArrow) && snake.direction.x == 0)
			snake.direction = new Vector2Int(-1, 0);
		if (Input.GetKeyDown(KeyCode.RightArrow) && snake.direction.x == 0)
			snake.direction = new Vector2Int(1, 0);
	}

	IEnumerator SnakeMovementRoutine()
	{
		while (true)
		{
			Vector2Int newHeadPosition = snake.segments[0] + snake.direction; // Calcula a nova posição da cabeça
			if (IsPositionInsideBoard(newHeadPosition)) // Verifica se a nova posição está dentro da matriz
			{
				snake.Move();
				CheckCollisions(); // Verifica as colisões antes de atualizar o tabuleiro
				UpdateGameBoard();
			}
			else
			{
				GameOver(); // Se a posição estiver fora da matriz, termina o jogo
			}

			yield return new WaitForSeconds(0.5f); // Velocidade da cobrinha.
		}
	}

	bool IsPositionInsideBoard(Vector2Int position)
	{
		return position.x >= 0 && position.x < 10 && position.y >= 0 && position.y < 20;
	}

	
	void CheckCollisions()
	{
		Vector2Int headPos = snake.segments[0];

		// Verifica se a cobrinha tenta sair da matriz
		if (headPos.x < 0 || headPos.x >= 10 || headPos.y < 0 || headPos.y >= 20)
		{
			GameOver();
			return;
		}

		// Verifica se a cobrinha colide consigo mesma
		for (int i = 1; i < snake.segments.Count; i++)
		{
			if (snake.segments[i] == headPos)
			{
				GameOver();
				return;
			}
		}

		// Verifica se a cobrinha comeu a maçã
		if (headPos == applePosition)
		{
			snake.Grow();
			PlaceApple();
            appleSound.Play();
            scoreTextMeshPro.text = "Score: " + snake.score;
		}
	}

	void UpdateGameBoard()
	{
		foreach (var block in boardBlocks.Values)
		{
			block.SetActive(false);
			block.GetComponent<SpriteRenderer>().color = snakeColor; // definindo a cor padr�o para todos os blocos
		}

		foreach (Vector2Int pos in snake.segments)
		{
			boardBlocks[pos].SetActive(true);
			boardBlocks[pos].GetComponent<SpriteRenderer>().color = snakeColor;
		}

		boardBlocks[applePosition].SetActive(true);
		boardBlocks[applePosition].GetComponent<SpriteRenderer>().color = appleColor;
	}


	void PlaceApple()
	{
		do
		{
			applePosition = new Vector2Int(Random.Range(0, 10), Random.Range(0, 20));
		} while (snake.segments.Contains(applePosition));

		boardBlocks[applePosition].SetActive(true);
	}

	public void RestartGame()
	{
		SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex); // Recarrega a cena atual
	}

	void GameOver()
	{
		gameOverPanel.SetActive(true);
		
		StopAllCoroutines();
		// TODO: Implemente a l�gica do GameOver aqui. Voc� pode mostrar uma mensagem, reiniciar o jogo, etc.
	}
}
