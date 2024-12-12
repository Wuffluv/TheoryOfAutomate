import tkinter as tk
import random

# Функция проверки, что координаты в пределах границ
def within_bounds(x, y, width, height):
    return 0 <= x < width and 0 <= y < height

# Класс для создания лабиринта с использованием клеточного автомата
class CellularAutomatonMaze:
    def __init__(self, master, width, height, cell_size):
        self.master = master
        self.width = width  # Ширина лабиринта в клетках
        self.height = height  # Высота лабиринта в клетках
        self.cell_size = cell_size  # Размер каждой клетки в пикселях
        self.grid = [[1 for _ in range(width)] for _ in range(height)]  #сетки лабиринта

        # отображение лабиринта
        self.canvas = tk.Canvas(master, width=width * cell_size, height=height * cell_size, bg="white")
        self.canvas.pack()

        # Изначально отрисовываем сетку пустых клеток
        for y in range(height):  # Проходим по строкам
            for x in range(width):  # Проходим по столбцам
                self.draw_cell(x, y, 1)  # Отрисовываем каждую клетку как стену

    # Метод для отрисовки ячейки
    def draw_cell(self, x, y, state):
        color = "white" if state == 1 else "black"

        self.canvas.create_rectangle(
            x * self.cell_size,
            y * self.cell_size,
            (x + 1) * self.cell_size,
            (y + 1) * self.cell_size,
            fill=color,
            outline="gray"
        )
        self.master.update()  # Обновляем экран для отображения изменений

    def initialize_grid(self):  # Метод для инициализации сетки (все клетки - стены)
        for y in range(self.height):  # Проходим по строкам
            for x in range(self.width):  # Проходим по столбцам
                self.grid[y][x] = 1  # Устанавливаем стену в каждой клетке
                self.draw_cell(x, y, 1)  # Отрисовываем каждую клетку как стену

# Метод для генерации лабиринта с клеточным автоматом
    def generate(self):
        self.initialize_grid()  # Инициализируем лабиринт как полностью заполненный стенами

        # Начинаем с центра лабиринта
        start_x, start_y = self.width // 2, self.height // 2  # Координаты начальной клетки
        self.grid[start_y][start_x] = 0  # Устанавливаем проход в стартовой клетке
        self.draw_cell(start_x, start_y, 0)  # Отрисовываем стартовую клетку как проход

        stack = [(start_x, start_y)]  # Стек для реализации алгоритма поиска в глубину

        # Сдвиги для соседей (вверх, вниз, влево, вправо)
        directions = [(-2, 0), (2, 0), (0, -2), (0, 2)]  # Возможные направления для перехода

        while stack:  # Пока стек не пуст
            x, y = stack[-1]  # Получаем координаты текущей клетки

            # Находим всех соседей, которые могут стать проходами
            neighbors = []  # Список соседей, которые могут стать проходами
            for dx, dy in directions:  # Перебираем возможные направления
                nx, ny = x + dx, y + dy  # Новые координаты соседа
                if within_bounds(nx, ny, self.width, self.height) and self.grid[ny][nx] == 1:  # Если сосед в пределах лабиринта и это стена
                    wall_x, wall_y = x + dx // 2, y + dy // 2  # Находим стену между текущей и соседней клеткой
                    if self.grid[wall_y][wall_x] == 1:  # Если стена еще не разрушена
                        neighbors.append((nx, ny, wall_x, wall_y))  # Добавляем соседа в список

            if neighbors:  # Если есть соседи
                # Выбираем случайного соседа
                nx, ny, wall_x, wall_y = random.choice(neighbors)  # Случайным образом выбираем соседа

                # Делаем проход через стену и переходим к соседу
                self.grid[wall_y][wall_x] = 0  # Преобразуем стену в проход
                self.draw_cell(wall_x, wall_y, 0)  # Отрисовываем стену как проход
                self.grid[ny][nx] = 0  # Преобразуем соседнюю клетку в проход
                self.draw_cell(nx, ny, 0)  # Отрисовываем соседнюю клетку как проход

                stack.append((nx, ny))  # Добавляем соседа в стек для дальнейшей работы
            else:  # Если нет соседей, возвращаемся назад по стеку
                stack.pop()  # Убираем текущую клетку из стека

        # Создаем вход и выход
        self.grid[1][self.width // 2] = 0  # Вход сверху
        self.grid[self.height - 2][self.width // 2] = 0  # Выход снизу
        self.draw_cell(self.width // 2, 1, 0)  # Отрисовываем вход
        self.draw_cell(self.width // 2, self.height - 2, 0)  # Отрисовываем выход

if __name__ == "__main__":

    WIDTH = 41
    HEIGHT = 41
    CELL_SIZE = 15

    root = tk.Tk()
    root.title("Генерация лабиринта клеточным автоматом")

    maze = CellularAutomatonMaze(root, WIDTH, HEIGHT, CELL_SIZE)
    maze.generate()  

    root.mainloop()
