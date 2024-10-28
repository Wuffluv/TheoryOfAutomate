import numpy as np
import matplotlib.pyplot as plt

# Описание состояний для вывода
state_descriptions = {
    0: "Кот не поел с утра",
    1: "Кот убежал из дома",
    2: "Кот уснул, забыв про еду",
    3: "Еда оказалась испорченной",
    4: "Кот застрял на дереве",
    5: "Кот пошёл охотиться на мышей",
    6: "Поймал мышь и поел",
    7: "Не поймал мышь, вернулся голодным",
    8: "Проснулся от голода",
    9: "Остался голодным",
    10: "Хозяин накормил его",
    11: "Проснулся, но решил поиграть",
}

# Матрица переходов для состояний кота
transition_matrix = np.array([
    [0, 0.5, 0.3, 0.2, 0, 0, 0, 0, 0, 0, 0, 0],  # Кот не поел с утра
    [0, 0, 0, 0, 0.5, 0.5, 0, 0, 0, 0, 0, 0],    # Кот убежал из дома
    [0, 0, 0, 0, 0, 0, 0, 0, 0.5, 0, 0.5, 0],    # Кот уснул, забыв про еду
    [0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0],        # Еда оказалась испорченной (поглощающее состояние)
    [0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1.0, 0],      # Кот застрял на дереве (поглощающее состояние)
    [0, 0, 0, 0, 0, 0, 0.5, 0.5, 0, 0, 0, 0],    # Кот пошёл охотиться на мышей
    [0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1.0, 0],      # Поймал мышь и поел (поглощающее состояние)
    [0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1.0, 0],      # Не поймал мышь, вернулся голодным (поглощающее состояние)
    [0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1.0],      # Проснулся от голода (поглощающее состояние)
    [0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1.0],      # Остался голодным (поглощающее состояние)
    [0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1.0],      # Хозяин накормил его (поглощающее состояние)
    [0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1.0]       # Проснулся, но решил поиграть (поглощающее состояние)
])

class CatBehaviorSimulation:
    def __init__(self, transition_matrix):
        # Инициализация симуляции с заданной матрицей переходов
        self.transition_matrix = transition_matrix
        self.num_states = transition_matrix.shape[0]  # Количество состояний

    # Обновление состояний из одного состояния в другое на основе вероятностей в матрцие
    def step(self, current_state):

        new_state = np.random.choice(self.num_states, p=self.transition_matrix[current_state])
        return new_state

    def simulate(self, start_state, num_simulations=10000):
        # Запускает симуляцию с начальным состоянием и возвращает время до поглощения и конечное состояние
        times_to_absorption = []  # Список для хранения времени до поглощения
        absorbing_states = []     # Список для хранения поглощающих состояний, в которые попадает кот

        for _ in range(num_simulations):
            current_state = start_state  # Начальное состояние
            time_steps = 0  # Счётчик шагов для текущей симуляции

            # Определение поглощающих состояний, из которых нет выхода
            absorbing = [3, 4, 6, 7, 9, 10, 11]

            # Выполняем шаги симуляции до тех пор, пока не достигнем поглощающего состояния
            while current_state not in absorbing:
                current_state = self.step(current_state)
                time_steps += 1

            times_to_absorption.append(time_steps)  # Запоминаем время до поглощения
            absorbing_states.append(current_state)  # Запоминаем конечное состояние

        return times_to_absorption, absorbing_states

def run(start_state, transition_matrix):
    """Запускает симуляцию и строит графики по результатам."""
    simulation = CatBehaviorSimulation(transition_matrix)  # Создаём экземпляр симуляции
    times, absorbing_states = simulation.simulate(start_state)  # Запуск симуляции

    # Расчёт среднего времени до поглощения
    average_time = np.mean(times)
    print(f"Среднее время до поглощения (начальное состояние '{state_descriptions[start_state]}'): {average_time:.2f} шага")

    # Расчёт вероятности достижения каждого поглощающего состояния
    unique, counts = np.unique(absorbing_states, return_counts=True)
    probabilities = dict(zip(unique, counts / len(absorbing_states)))

    # Выводим вероятность для каждого конечного состояния
    for state, prob in probabilities.items():
        print(f"Вероятность достичь состояния '{state_descriptions[state]}': {prob:.2f}")

    # Построение гистограммы времени до поглощения
    plt.hist(times, bins=30, color='skyblue', edgecolor='black')
    plt.title(f"Гистограмма времени до поглощения (начальное состояние '{state_descriptions[start_state]}')")
    plt.xlabel("Число шагов до поглощения")
    plt.ylabel("Частота")
    plt.show()

# Запуск симуляции для начального состояния "Кот не поел с утра"
run(0, transition_matrix)

# Запуск симуляции для состояния "Кот уснул, забыв про еду"
run(2, transition_matrix)
