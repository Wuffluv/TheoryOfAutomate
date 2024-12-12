import tkinter as tk
import random
import time

class MazeGenerator:
    def __init__(self, master, width, height, cell_size):
        self.master = master
        self.width = width
        self.height = height
        self.cell_size = cell_size
        self.grid = [[1 for _ in range(width)] for _ in range(height)]


        self.canvas = tk.Canvas(master, width=width * cell_size, height=height * cell_size, bg="white")
        self.canvas.pack()

        # Изначально отрисовываем сетку пустых клеток
        for y in range(height):
            for x in range(width):
                self.draw_cell(x, y, 1)

    def draw_cell(self, x, y, state):
        """
        Рисует ячейку в заданных координатах.
        state: 0 - проход, 1 - стена
        """
        color = "white" if state == 1 else "black"
        self.canvas.create_rectangle(
            x * self.cell_size,
            y * self.cell_size,
            (x + 1) * self.cell_size,
            (y + 1) * self.cell_size,
            fill=color,
            outline="gray"
        )
        self.master.update()

    def generate(self):
        """
        Генерирует лабиринт пошагово, отображая процесс в реальном времени.
        Использует алгоритм случайного блуждания с входом и выходом.
        """
        # Устанавливаем вход и выход
        self.grid[0][1] = 0
        self.draw_cell(1, 0, 0)
        self.grid[self.height - 1][self.width - 2] = 0
        self.draw_cell(self.width - 2, self.height - 1, 0)

        # Начальная точка для генерации
        start_x, start_y = 1, 0
        self.grid[start_y][start_x] = 0

        # Список границ (стен, которые могут быть удалены)
        walls = [(nx, ny, x, y) for nx, ny, x, y in self.get_neighbors(start_x, start_y, include_walls=True)]

        while walls:
            wx, wy, nx, ny = random.choice(walls)
            walls.remove((wx, wy, nx, ny))

            if self.grid[ny][nx] == 1:  # Если соседняя ячейка ещё не посещена
                # Удаляем стену
                self.grid[wy][wx] = 0
                self.grid[ny][nx] = 0
                self.draw_cell(wx, wy, 0)
                self.draw_cell(nx, ny, 0)

                # Добавляем новые стены
                for next_wall in self.get_neighbors(nx, ny, include_walls=True):
                    walls.append(next_wall)

            time.sleep(0.01)  # Задержка для визуализации

    def get_neighbors(self, x, y, include_walls=False):
        """
        Возвращает соседей для текущей ячейки.
        include_walls: если True, возвращает также координаты стен.
        """
        directions = [(-2, 0), (2, 0), (0, -2), (0, 2)]
        neighbors = []

        for dx, dy in directions:
            nx, ny = x + dx, y + dy
            if 0 <= nx < self.width and 0 <= ny < self.height:
                wall_x, wall_y = x + dx // 2, y + dy // 2
                if include_walls or self.grid[ny][nx] == 1:
                    neighbors.append((wall_x, wall_y, nx, ny))

        return neighbors

if __name__ == "__main__":
    # Параметры лабиринта
    WIDTH = 21  # Число клеток по горизонтали (должно быть нечётным)
    HEIGHT = 21  # Число клеток по вертикали (должно быть нечётным)
    CELL_SIZE = 20

    root = tk.Tk()
    root.title("Генератор лабиринта")

    maze = MazeGenerator(root, WIDTH, HEIGHT, CELL_SIZE)
    maze.generate()

    root.mainloop()
