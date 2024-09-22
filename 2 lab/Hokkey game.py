import sys
sys.path.append('C:/Users/wolfd/AppData/Roaming/Python/Python38/site-packages')
import pygame
import random
import math

# Определение цветов
WHITE = (255, 255, 255)
BLACK = (0, 0, 0)
RED = (255, 0, 0)
BLUE = (0, 0, 255)

# Размер окна
WINDOW_WIDTH = 800
WINDOW_HEIGHT = 600

# Класс для 2D вектора
class Vector2D:
    def __init__(self, x=0, y=0):
        self.x = x
        self.y = y

    def add(self, other):
        self.x += other.x
        self.y += other.y

    def length(self):
        return math.sqrt(self.x**2 + self.y**2)

    def normalize(self):
        length = self.length()
        if length > 0:
            self.x /= length
            self.y /= length

    def scale(self, scalar):
        self.x *= scalar
        self.y *= scalar

# Класс для хоккеистов
class Athlete:
    def __init__(self, x, y, color, goal_position):
        self.position = Vector2D(x, y)
        self.velocity = Vector2D(random.uniform(-1, 1), random.uniform(-1, 1))
        self.velocity.normalize()
        self.velocity.scale(4)  # Увеличиваем скорость
        self.color = color
        self.radius = 15
        self.goal_position = goal_position  # Позиция ворот противника

    def update(self, puck):
        if puck.owner == self:
            # Если хоккеист владеет шайбой, он движется к воротам противника
            direction = Vector2D(self.goal_position.x - self.position.x, self.goal_position.y - self.position.y)
            direction.normalize()
            direction.scale(3)  # Скорость движения к воротам
            self.position.add(direction)
        else:
            # Простой ИИ для преследования шайбы
            direction = Vector2D(puck.position.x - self.position.x, puck.position.y - self.position.y)
            direction.normalize()
            direction.scale(2)  # Скорость преследования шайбы
            self.position.add(direction)

    def draw(self, screen):
        pygame.draw.circle(screen, self.color, (int(self.position.x), int(self.position.y)), self.radius)

# Класс для шайбы
class Puck:
    def __init__(self, x, y):
        self.position = Vector2D(x, y)
        self.velocity = Vector2D(random.uniform(-2, 2), random.uniform(-2, 2))
        self.velocity.normalize()
        self.velocity.scale(4)
        self.radius = 10
        self.owner = None

    def update(self):
        if self.owner is None:
            self.position.add(self.velocity)
            self.check_bounds()

    def check_bounds(self):
        if self.position.x < 0 or self.position.x > WINDOW_WIDTH:
            self.velocity.x = -self.velocity.x
        if self.position.y < 0 or self.position.y > WINDOW_HEIGHT:
            self.velocity.y = -self.velocity.y

    def draw(self, screen):
        pygame.draw.circle(screen, BLACK, (int(self.position.x), int(self.position.y)), self.radius)

# Класс для базового состояния
class GameState:
    def update(self):
        pass

    def draw(self, screen):
        pass

    def handle_input(self, event):
        pass

# Класс состояния игры
class PlayState(GameState):
    def __init__(self):
        self.athletes = [
            Athlete(100, 100, RED, Vector2D(WINDOW_WIDTH - 20, WINDOW_HEIGHT // 2)),
            Athlete(700, 100, BLUE, Vector2D(10, WINDOW_HEIGHT // 2)),
            Athlete(100, 500, RED, Vector2D(WINDOW_WIDTH - 20, WINDOW_HEIGHT // 2)),
            Athlete(700, 500, BLUE, Vector2D(10, WINDOW_HEIGHT // 2))
        ]
        self.puck = Puck(WINDOW_WIDTH // 2, WINDOW_HEIGHT // 2)
        self.right_goal = pygame.Rect(WINDOW_WIDTH - 20, WINDOW_HEIGHT // 2 - 50, 10, 100)
        self.left_goal = pygame.Rect(10, WINDOW_HEIGHT // 2 - 50, 10, 100)
        self.score_red = 0
        self.score_blue = 0

    def update(self):
        self.puck.update()
        for athlete in self.athletes:
            athlete.update(self.puck)

        for athlete in self.athletes:
            if self.puck.owner is None and self.distance(athlete.position, self.puck.position) < athlete.radius:
                self.puck.owner = athlete
                break

        self.check_goal()

    def check_goal(self):
        if self.left_goal.collidepoint(self.puck.position.x, self.puck.position.y):
            if self.puck.owner and self.puck.owner.color == BLUE:
                self.score_blue += 1
            self.reset_game()

        if self.right_goal.collidepoint(self.puck.position.x, self.puck.position.y):
            if self.puck.owner and self.puck.owner.color == RED:
                self.score_red += 1
            self.reset_game()

    def reset_game(self):
        self.puck.position = Vector2D(WINDOW_WIDTH // 2, WINDOW_HEIGHT // 2)
        self.puck.owner = None
        self.puck.velocity = Vector2D(random.uniform(-2, 2), random.uniform(-2, 2))
        self.puck.velocity.normalize()
        self.puck.velocity.scale(4)

    def distance(self, pos1, pos2):
        return math.sqrt((pos1.x - pos2.x)**2 + (pos1.y - pos2.y)**2)

    def draw(self, screen):
        pygame.draw.rect(screen, WHITE, self.right_goal)
        pygame.draw.rect(screen, WHITE, self.left_goal)
        for athlete in self.athletes:
            athlete.draw(screen)
        self.puck.draw(screen)
        font = pygame.font.Font(None, 36)
        score_text = font.render(f"Red: {self.score_red} Blue: {self.score_blue}", True, BLACK)
        screen.blit(score_text, (WINDOW_WIDTH // 2 - 100, 20))

# Класс управления состояниями игры (стек состояний)
class GameStateManager:
    def __init__(self):
        self.stack = []

    def push(self, state):
        self.stack.append(state)

    def pop(self):
        if len(self.stack) > 0:
            self.stack.pop()

    def update(self):
        if len(self.stack) > 0:
            self.stack[-1].update()

    def draw(self, screen):
        if len(self.stack) > 0:
            self.stack[-1].draw(screen)

    def handle_input(self, event):
        if len(self.stack) > 0:
            self.stack[-1].handle_input(event)

# Главный цикл игры
def main():
    pygame.init()
    screen = pygame.display.set_mode((WINDOW_WIDTH, WINDOW_HEIGHT))
    pygame.display.set_caption("Hockey Game")
    clock = pygame.time.Clock()

    # Инициализация менеджера состояний
    game_state_manager = GameStateManager()

    # Добавляем игровое состояние в стек
    game_state_manager.push(PlayState())

    running = True
    while running:
        screen.fill((0, 128, 128))

        # Обработка событий
        for event in pygame.event.get():
            if event.type == pygame.QUIT:
                running = False
            game_state_manager.handle_input(event)

        # Обновление игрового состояния
        game_state_manager.update()

        # Отображение элементов на экране
        game_state_manager.draw(screen)

        # Обновление экрана
        pygame.display.flip()

        clock.tick(60)

    pygame.quit()

if __name__ == "__main__":
    main()
