import sys
sys.path.append('C:/Users/wolfd/AppData/Roaming/Python/Python38/site-packages')
import pygame
import random
import math

# Константы
SCREEN_WIDTH, SCREEN_HEIGHT = 800, 600
PLAYER_RADIUS = 15
PUCK_RADIUS = 10
PLAYER_SPEED = 5
PUCK_SPEED = 7

# Векторные вычисления
def distance(v1, v2):
    return math.sqrt((v2[0] - v1[0]) ** 2 + (v2[1] - v1[1]) ** 2)

def normalize(vector):
    magnitude = math.sqrt(vector[0] ** 2 + vector[1] ** 2)
    if magnitude == 0:
        return (0, 0)
    return (vector[0] / magnitude, vector[1] / magnitude)

def vector_add(v1, v2):
    return (v1[0] + v2[0], v1[1] + v2[1])

def vector_sub(v1, v2):
    return (v1[0] - v2[0], v1[1] - v2[1])

def vector_scale(vector, scale):
    return (vector[0] * scale, vector[1] * scale)

# Класс хоккеиста
class Athlete:
    def __init__(self, x, y, team):
        self.position = [x, y]
        self.team = team
        self.velocity = [0, 0]
        self.has_puck = False

    def update(self, puck, screen_width, screen_height):
        # Если игрок владеет шайбой, он движется с ней
        if self.has_puck:
            puck.position = vector_add(self.position, (PUCK_RADIUS + PLAYER_RADIUS, 0))

        # Плавное перемещение
        self.position[0] = max(PLAYER_RADIUS, min(self.position[0] + self.velocity[0], screen_width - PLAYER_RADIUS))
        self.position[1] = max(PLAYER_RADIUS, min(self.position[1] + self.velocity[1], screen_height - PLAYER_RADIUS))

    def seek(self, target):
        # Направляем игрока к цели
        desired_velocity = vector_sub(target, self.position)
        desired_velocity = normalize(desired_velocity)
        self.velocity = vector_scale(desired_velocity, PLAYER_SPEED)

    def wander(self):
        # Блуждание по катку
        wander_target = [random.randint(0, SCREEN_WIDTH), random.randint(0, SCREEN_HEIGHT)]
        self.seek(wander_target)

    def draw(self, screen):
        color = (0, 0, 255) if self.team == 'blue' else (255, 0, 0)
        pygame.draw.circle(screen, color, (int(self.position[0]), int(self.position[1])), PLAYER_RADIUS)

# Класс шайбы
class Puck:
    def __init__(self):
        self.position = [SCREEN_WIDTH // 2, SCREEN_HEIGHT // 2]
        self.velocity = [0, 0]

    def update(self):
        self.position[0] += self.velocity[0]
        self.position[1] += self.velocity[1]

        # Ограничение шайбы полем
        if self.position[0] < PUCK_RADIUS or self.position[0] > SCREEN_WIDTH - PUCK_RADIUS:
            self.velocity[0] = -self.velocity[0]
        if self.position[1] < PUCK_RADIUS or self.position[1] > SCREEN_HEIGHT - PUCK_RADIUS:
            self.velocity[1] = -self.velocity[1]

    def draw(self, screen):
        pygame.draw.circle(screen, (0, 0, 0), (int(self.position[0]), int(self.position[1])), PUCK_RADIUS)

# Основной класс игры
class HockeyGame:
    def __init__(self):
        pygame.init()
        self.screen = pygame.display.set_mode((SCREEN_WIDTH, SCREEN_HEIGHT))
        pygame.display.set_caption("Hockey Game")
        self.clock = pygame.time.Clock()

        self.players = [
            Athlete(100, 100, 'blue'),
            Athlete(700, 100, 'red'),
        ]
        self.puck = Puck()

    def run(self):
        running = True
        while running:
            self.clock.tick(60)
            for event in pygame.event.get():
                if event.type == pygame.QUIT:
                    running = False

            self.update()
            self.draw()

        pygame.quit()

    def update(self):
        keys = pygame.key.get_pressed()

        if keys[pygame.K_w]:
            self.players[0].seek(self.puck.position)
        else:
            self.players[0].wander()

        if keys[pygame.K_s]:
            self.players[1].seek(self.puck.position)
        else:
            self.players[1].wander()

        for player in self.players:
            player.update(self.puck, SCREEN_WIDTH, SCREEN_HEIGHT)

        self.puck.update()

    def draw(self):
        self.screen.fill((255, 255, 255))

        for player in self.players:
            player.draw(self.screen)

        self.puck.draw(self.screen)
        pygame.display.flip()

# Запуск игры
if __name__ == "__main__":
    game = HockeyGame()
    game.run()
