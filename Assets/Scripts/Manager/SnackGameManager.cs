using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using TMPro;

public class SnakeGameManager : MonoBehaviour
{
    // Variáveis públicas para a interface e outros elementos do jogo
    public GameObject gameOverPanel;
    public TextMeshProUGUI scoreTextMeshPro;
    public GameObject blockPrefab;
    public Color snakeColor = new Color(1f, 1f, 1f, 0.8f); // Cor da cobra
    public Color appleColor = new Color(1f, 0f, 0f, 0.8f); // Cor da maçã

    // Variáveis para controlar sons
    public AudioSource backgroundMusic;
    public AudioSource appleSound;

    // Variáveis privadas para controle interno do jogo
    private Vector2[,] positions;
    private float startX = -1.596f;
    private float startY = 0.211f;
    private float offset = 0.211f;

    private Snake snake;
    private Vector2Int applePosition;

    // Estrutura para manter os blocos do tabuleiro
    private Dictionary<Vector2Int, GameObject> boardBlocks = new Dictionary<Vector2Int, GameObject>();

    void Start()
    {
        // Inicializações
        InitializeMatrix();
        InitializeSnake();
        PlaceApple();
        StartCoroutine(SnakeMovementRoutine());
        backgroundMusic.Play();  // Iniciar música de fundo
    }

    // Inicializa a matriz de posições do jogo
    void InitializeMatrix()
    {
        positions = new Vector2[10, 20];

        for (int x = 0; x < 10; x++)
        {
            for (int y = 0; y < 20; y++)
            {
                positions[x, y] = new Vector2(startX + x * offset, startY + y * offset);

                GameObject block = Instantiate(blockPrefab, positions[x, y], Quaternion.identity);
                block.SetActive(false);  // O bloco é inicialmente invisível

                boardBlocks[new Vector2Int(x, y)] = block;
            }
        }
    }

    // Inicializa a cobra no meio do tabuleiro
    void InitializeSnake()
    {
        snake = new Snake(new Vector2Int(5, 10), new Vector2Int(1, 0));
    }

    // Atualiza o jogo a cada quadro
    void Update()
    {
        HandleInput();

        // Se a cobra come a maçã
        if (snake.segments[0] == applePosition)
        {
            snake.Grow();
            PlaceApple();
        }
    }

    // Controla a entrada do usuário
    void HandleInput()
    {
        // Código para mudar a direção da cobra
        // Observação: a cobra não pode inverter imediatamente a sua direção
        if (Input.GetKeyDown(KeyCode.UpArrow) && snake.direction.y == 0)
            snake.direction = new Vector2Int(0, 1);
        if (Input.GetKeyDown(KeyCode.DownArrow) && snake.direction.y == 0)
            snake.direction = new Vector2Int(0, -1);
        if (Input.GetKeyDown(KeyCode.LeftArrow) && snake.direction.x == 0)
            snake.direction = new Vector2Int(-1, 0);
        if (Input.GetKeyDown(KeyCode.RightArrow) && snake.direction.x == 0)
            snake.direction = new Vector2Int(1, 0);
    }

    // Coroutine para mover a cobra
    IEnumerator SnakeMovementRoutine()
    {
        while (true)
        {
            Vector2Int newHeadPosition = snake.segments[0] + snake.direction;

            // Se a nova posição é válida, mover a cobra
            if (IsPositionInsideBoard(newHeadPosition))
            {
                snake.Move();
                CheckCollisions();
                UpdateGameBoard();
            }
            else
            {
                GameOver();
            }

            yield return new WaitForSeconds(0.5f);  // Delay para o próximo movimento
        }
    }

    // Verifica se a posição está dentro dos limites do tabuleiro
    bool IsPositionInsideBoard(Vector2Int position)
    {
        return position.x >= 0 && position.x < 10 && position.y >= 0 && position.y < 20;
    }

    // Verifica se há colisões
    void CheckCollisions()
    {
        Vector2Int headPos = snake.segments[0];

        // Verifica se a cobra saiu do tabuleiro
        if (!IsPositionInsideBoard(headPos))
        {
            GameOver();
            return;
        }

        // Verifica se a cobra colidiu consigo mesma
        for (int i = 1; i < snake.segments.Count; i++)
        {
            if (snake.segments[i] == headPos)
            {
                GameOver();
                return;
            }
        }

        // Verifica se a cobra comeu a maçã
        if (headPos == applePosition)
        {
            snake.Grow();
            PlaceApple();
            appleSound.Play();  // Tocar som da maçã sendo comida
            scoreTextMeshPro.text = "Score: " + snake.score;  // Atualizar o placar
        }
    }

    // Atualiza o tabuleiro com as posições da cobra e da maçã
    void UpdateGameBoard()
    {
        // Desativar todos os blocos e definir a cor padrão
        foreach (var block in boardBlocks.Values)
        {
            block.SetActive(false);
            block.GetComponent<SpriteRenderer>().color = snakeColor;
        }

        // Ativar e colorir os blocos onde a cobra está
        foreach (Vector2Int pos in snake.segments)
        {
            boardBlocks[pos].SetActive(true);
            boardBlocks[pos].GetComponent<SpriteRenderer>().color = snakeColor;
        }

        // Ativar e colorir o bloco onde a maçã está
        boardBlocks[applePosition].SetActive(true);
        boardBlocks[applePosition].GetComponent<SpriteRenderer>().color = appleColor;
    }

    // Posiciona uma nova maçã em uma posição aleatória que não seja ocupada pela cobra
    void PlaceApple()
    {
        do
        {
            applePosition = new Vector2Int(Random.Range(0, 10), Random.Range(0, 20));
        } while (snake.segments.Contains(applePosition));

        boardBlocks[applePosition].SetActive(true);
    }

    // Reinicia o jogo
    public void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    // Termina o jogo e mostra o painel de Game Over
    void GameOver()
    {
        gameOverPanel.SetActive(true);
        StopAllCoroutines();  // Para todas as corrotinas para interromper o movimento da cobra
    }
}
