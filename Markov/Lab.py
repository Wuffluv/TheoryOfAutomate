import numpy as np
import matplotlib.pyplot as plt

# Определение матрицы переходов для состояний кота
transition_matrix = np.array([
    [0, 0.5, 0.3, 0.2, 0, 0, 0, 0, 0, 0, 0],  # Кот не поел с утра (начальное состояние)
    [0, 0, 0, 0, 1.0, 0, 0, 0, 0, 0, 0],      # Кот застрял на дереве
    [0, 0, 0, 0, 0, 0, 0, 0, 1.0, 0, 0],      # Проснулся от голода
    [0, 0, 0, 0, 0, 0, 0, 0, 0, 1.0, 0],      # Кот попытался поесть, но не смог
    [0, 0, 0, 0, 0, 1.0, 0, 0, 0, 0, 0],      # Кот мяукает, ожидая помощи (поглощающее состояние)
    [0, 0, 0, 0, 0, 0, 0.5, 0.5, 0, 0, 0],    # Кот пошёл охотиться на мышей
    [0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1.0],      # Поймал мышь и поел (поглощающее состояние)
    [0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1.0],      # Не поймал мышь, вернулся голодным (поглощающее состояние)
    [0, 0, 0, 0, 0, 0, 0, 0.3, 0.7, 0, 0],    # Проснулся, но решил поиграть
    [0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1.0],      # Остался голодным (поглощающее состояние)
    [0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1.0]       # Хозяин накормили его (поглощающее состояние)
])

# текстовое описание каждого состояния для вывода
state_descriptions = {
    0: "Кот не поел с утра",
    1: "Кот застрял на дереве",
    2: "Проснулся от голода",
    3: "Кот попытался поесть, но не смог",
    4: "Кот мяукает, ожидая помощи",
    5: "Кот пошёл охотиться на мышей",
    6: "Поймал мышь и поел",
    7: "Не поймал мышь, вернулся голодным",
    8: "Проснулся, но решил поиграть",
    9: "Остался голодным",
    10: "Хозяин накормили его"
}

class CatBehaviorSimulation:
    def __init__(self, transition_matrix):
        # Инициализация симуляции с заданной матрицей переходов
        self.transition_matrix = transition_matrix  # Сохраняем переданную матрицу переходов
        self.num_states = transition_matrix.shape[0]  # Определяем количество состояний

    def step(self, current_state):
        """Выполняет один шаг симуляции, выбирая новое состояние на основе текущего."""
        # Случайно выбираем следующее состояние с вероятностями из матрицы переходов
        new_state = np.random.choice(self.num_states, p=self.transition_matrix[current_state])
        return new_state

    def simulate(self, start_state, num_simulations=10000):
        """Запускает симуляцию, возвращает время до поглощения и конечное состояние."""
        times_to_absorption = []  # Список для хранения времени до поглощения
        absorbing_states = []     # Список для хранения достигнутых поглощающих состояний

        for _ in range(num_simulations):
            current_state = start_state  # Устанавливаем начальное состояние
            time_steps = 0  # Обнуляем количество шагов для текущей симуляции

            # Список поглощающих состояний — состояния, из которых нет выхода
            absorbing = [4, 6, 7, 9, 10]

            # Выполняем шаги до тех пор, пока не достигнем поглощающего состояния
            while current_state not in absorbing:
                current_state = self.step(current_state)
                time_steps += 1

            times_to_absorption.append(time_steps)  # Сохраняем время до поглощения
            absorbing_states.append(current_state)  # Сохраняем конечное состояние

        return times_to_absorption, absorbing_states

# Функция для запуска симуляции и построения графиков
def run_and_visualize_cat_simulation(start_state, transition_matrix):
    simulation = CatBehaviorSimulation(transition_matrix)  # Создаем экземпляр симуляции

    # Запускаем симуляцию и сохраняем результаты
    times, absorbing_states = simulation.simulate(start_state)

    # Вычисляем среднее время до поглощения
    average_time = np.mean(times)
    print(f"Среднее время до поглощения (начальное состояние '{state_descriptions[start_state]}'): {average_time:.2f} шага")

    # Вычисляем вероятность достижения каждого поглощающего состояния
    unique, counts = np.unique(absorbing_states, return_counts=True)
    probabilities = dict(zip(unique, counts / len(absorbing_states)))

    # Выводим вероятность для каждого конечного состояния с текстовым описанием
    for state, prob in probabilities.items():
        print(f"Вероятность достичь состояния '{state_descriptions[state]}': {prob:.2f}")

    # Построение гистограммы времени до поглощения
    plt.hist(times, bins=30, color='skyblue', edgecolor='black')
    plt.title(f"Гистограмма времени до поглощения (начальное состояние '{state_descriptions[start_state]}')")
    plt.xlabel("Число шагов до поглощения")
    plt.ylabel("Частота")
    plt.show()

# Запуск симуляции для начального состояния "Кот не поел с утра"
run_and_visualize_cat_simulation(0, transition_matrix)

# Запуск симуляции для состояния "Кот уснул, забыв про еду"
run_and_visualize_cat_simulation(2, transition_matrix)
